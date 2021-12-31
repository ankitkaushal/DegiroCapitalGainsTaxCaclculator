namespace DegiroTax
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Windows.Forms;
    using DegiroTax.Classes;
    using DegiroTax.Extensions;
    using DegiroTax.PostProcessors;
    using DegiroTax.Processors;
    using DegiroTax.Services;
    using DegiroTax.TransactionParser;

    public partial class Form1 : System.Windows.Forms.Form
    {
        private string filePath;

        public Form1()
        {
            this.InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            // inputs
            if (this.filePath == null)
            {
                MessageBox.Show("Please select document first.");
                return;
            }

            int taxYear;
            bool validTaxYear = int.TryParse(this.textBox1.Text, out taxYear);

            if (!validTaxYear || !(taxYear > 1980 && taxYear < 2030))
            {
                MessageBox.Show("Tax year must be a valid number and between 1980 and 2030");
                return;
            }

            bool washSaleRule = this.checkBox1.Checked;

            // Clear Grid and tax outptut
            this.dataGridView1.Rows.Clear();
            this.textBox5.Text = string.Empty;

            // Parse Transaction
            ITransactionParser parser = new CSVTransactionParser(this.filePath);
            var transactions = parser.Parse();

            // Sort transactions by date
            var orderedTransactions = transactions.OrderBy(t => t.DateTime).ToList();

            // Normalize stock split transactions
            ITransactionProcessor processor = new StockSplitProcessor();
            orderedTransactions = processor.Process(orderedTransactions);

            // Calculate tax
            ITaxCalculator calculator = new TaxCalculator(taxYear);

            List<ProfitAndLossTransaction> profitAndLossTransactions = calculator.CalculateProfitAndLoss(orderedTransactions);

            IPostProcessor postProcessor = new PostProcessor();

            if (washSaleRule)
            {
                profitAndLossTransactions = postProcessor.Process(profitAndLossTransactions);
            }

            double profit = calculator.CalculateTax(profitAndLossTransactions);

            this.PopulateGrid(profitAndLossTransactions, profit);
            this.PopulateTaxTextboxes(profit);
        }

        private void PopulateTaxTextboxes(double profit)
        {
            double exemption;
            if (!double.TryParse(this.textBox3.Text, out exemption))
            {
                MessageBox.Show("Exemption limit must be a valid number");
                return;
            }

            double rate;
            if (!double.TryParse(this.textBox4.Text, out rate))
            {
                MessageBox.Show("Rate must be a valid number");
                return;
            }

            this.textBox2.Text = profit.RoundOff().ToString();
            var taxableProfit = Math.Max(0.0, profit - exemption);
            var tax = taxableProfit * rate / 100.0;

            this.textBox5.Text = tax.ToString();
        }

        private void PopulateGrid(List<ProfitAndLossTransaction> profitAndLossTransactions, double profit)
        {
            var count = 0;
            foreach (var pnlTransaction in profitAndLossTransactions)
            {
                this.dataGridView1.Rows.Add();
                this.dataGridView1[0, count].Value = pnlTransaction.BuyTransaction.Stock.Name;
                this.dataGridView1[1, count].Value = pnlTransaction.BuyTransaction.DateTime;
                this.dataGridView1[2, count].Value = pnlTransaction.BuyTransaction.Price.RoundOff();
                this.dataGridView1[3, count].Value = pnlTransaction.SellTransaction.DateTime;
                this.dataGridView1[4, count].Value = pnlTransaction.SellTransaction.Price.RoundOff();
                this.dataGridView1[5, count].Value = -pnlTransaction.SellTransaction.Quantity.RoundOff();
                this.dataGridView1[6, count].Value = pnlTransaction.ActualProfit().RoundOff();

                if (pnlTransaction.IsWashSale())
                {
                    this.dataGridView1[7, count].Value = pnlTransaction.Profit.RoundOff();
                    this.dataGridView1.Rows[count].DefaultCellStyle.BackColor = Color.LightYellow;
                }

                count += 1;
            }

            this.dataGridView1[6, count].Value = profit.RoundOff();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            // To where your opendialog box get starting location. My initial directory location is desktop.
            this.openFileDialog1.InitialDirectory = "C://Desktop";

            // Your opendialog box title name.
            this.openFileDialog1.Title = "Select file.";

            // which type file format you want to upload in database. just add them.
            this.openFileDialog1.Filter = "Select Valid Document(*.csv)|*.csv";

            // FilterIndex property represents the index of the filter currently selected in the file dialog box.
            this.openFileDialog1.FilterIndex = 1;
            try
            {
                if (this.openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    if (this.openFileDialog1.CheckFileExists)
                    {
                        this.filePath = System.IO.Path.GetFullPath(this.openFileDialog1.FileName);
                        this.label1.Text = "selected file " + this.filePath;
                    }
                }
                else
                {
                    MessageBox.Show("Please select document.");
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Unexpected error");
            }
        }
    }
}
