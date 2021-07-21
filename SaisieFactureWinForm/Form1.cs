using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SaisieFactureWinForm
{
    public partial class Form1 : Form
    {
        private bool _txbNumeroUpdated = false;
        private bool _txbMontantUpdated = false;
        private bool _noPaint = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Numéro");
            dataTable.Columns.Add("Montant");
            dataTable.Columns.Add("Date");

            dgvFactures.DataSource = dataTable;
        }

        private void btnAjouter_Click(object sender, EventArgs e)
        {
            DataTable dataTable = (DataTable)dgvFactures.DataSource;
            _txbNumeroUpdated = true;
            _txbMontantUpdated = true;
            var newFacture = RetournerFacture(out bool numeroNonValide, out bool montantNonValide);
            if (newFacture == null)
            {
                //MessageBox.Show("Un ou plusieurs champs saisi(s) sont incorrects");
                this.Refresh();
                return;
            }

            DataRow dataRow = dataTable.NewRow();
            dataTable.Rows.Add(dataRow);
            dataRow["Numéro"] = newFacture.Numero;
            dataRow["Montant"] = newFacture.Montant;
            dataRow["Date"] = newFacture.Date;

            _noPaint = true;
            txbNuméro.Text = "";
            txbMontant.Text = "";
            _txbNumeroUpdated = false;
            _txbMontantUpdated = false;
            txbMontant.BorderStyle = BorderStyle.FixedSingle;
            txbNuméro.BorderStyle = BorderStyle.FixedSingle;
            _noPaint = false;
            this.Refresh();
        }
        private void txbNuméro_TextChanged(object sender, EventArgs e)
        {
            _txbNumeroUpdated = true;
            this.Refresh();
        }

        private void txbMontant_TextChanged(object sender, EventArgs e)
        {
            _txbMontantUpdated = true;
            this.Refresh();
        }


        private Factures RetournerFacture(out bool numeroNonValide, out bool montantNonValide)
        {
            bool isMontantDecimal = IsMontantValide(out decimal montant);

            if (isMontantDecimal && IsNumeroValide())
            {
                var facture = new Factures();
                facture.Montant = montant;

                facture.Numero = txbNuméro.Text;
                facture.Date = dtpDate.Value;
                montantNonValide = false;
                numeroNonValide = false;

                return facture;
            }

            montantNonValide = !isMontantDecimal;
            numeroNonValide = !IsNumeroValide();
            return null;
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            if (_noPaint)
            {
                return;
            }

            if (IsNumeroValide())
            {
                txbNuméro.BorderStyle = BorderStyle.FixedSingle;
            }
            else if(_txbNumeroUpdated)
            {
                txbNuméro.BorderStyle = BorderStyle.None;
                Pen p = new Pen(Color.Red);
                Graphics g = e.Graphics;
                int variance = 3;
                g.DrawRectangle(p, new Rectangle(txbNuméro.Location.X - variance, txbNuméro.Location.Y - variance, 
                    txbNuméro.Width + variance, txbNuméro.Height + variance));
            }

            if (IsMontantValide(out decimal montant))
            {
                txbMontant.BorderStyle = BorderStyle.FixedSingle;
            }
            else if(_txbMontantUpdated)
            {
                txbMontant.BorderStyle = BorderStyle.None;
                Pen p = new Pen(Color.Red);
                Graphics g = e.Graphics;
                int variance = 3;
                g.DrawRectangle(p, new Rectangle(txbMontant.Location.X - variance, txbMontant.Location.Y - variance,
                    txbMontant.Width + variance, txbMontant.Height + variance));
            }
           
        }

        private bool IsMontantValide(out decimal montant)
        {
            string montantAsText = txbMontant.Text;
            montantAsText = montantAsText.Replace(".", ",");
            return decimal.TryParse(montantAsText, out montant);
            
        }
        private bool IsNumeroValide()
        {
            return !string.IsNullOrWhiteSpace(txbNuméro.Text);
        }

    }
}
