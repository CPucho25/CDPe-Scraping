using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace CDPe_Scraping
{
    public partial class Form2 : Form
    {
        private static IWebDriver driver;

        public Form2()
        {
            InitializeComponent();
            driver = new ChromeDriver();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            Thread.Sleep(5000);
            string url = @"https://ww1.sunat.gob.pe/ol-ti-itconsultaunificadalibre/consultaUnificadaLibre/consulta";
            driver.Url = url;
        }

        private void button1_Click(object sender, EventArgs e)
        {

            Thread.Sleep(4000);

            #region LeerArchivoTxt

            string currentDir_CDP = Environment.CurrentDirectory;
            string path_CDP = currentDir_CDP + @"\CDPs\";


            if (!Directory.Exists(path_CDP))
                Directory.CreateDirectory(path_CDP);

            DirectoryInfo di = new DirectoryInfo(path_CDP);


            int numFiles = 0;
            string numRow = "";

            foreach (var fi in di.GetFiles())
            {
                string path_CDP_txt = path_CDP + fi.Name;


                StreamReader read = File.OpenText(path_CDP_txt);

                string linea = read.ReadLine();

                string ruc;
                string tip_CDP;
                string serie_CDP;
                string num_CDP;
                string fe_CDP;
                string imp_tot_CDP;

                string estado1, estado2, estado3, resEstados;
                string MsgError = "";
                bool elementError = false;

                int i = 0;
                int numerointentos = 0;
                bool escritutaCorrecta = false;

                #region Recorrer txt linea x linea

                while (linea != null)
                {
                    string[] campos = new string[6];
                    campos = linea.Split('|');

                    ruc = campos[0];
                    tip_CDP = campos[1];
                    serie_CDP = campos[2];
                    num_CDP = campos[3];
                    fe_CDP = campos[4];
                    imp_tot_CDP = campos[5];

                    var ERRORmSG = "";
                    numerointentos = 1;
                    escritutaCorrecta = false;

                    while (numerointentos <= 4 && escritutaCorrecta == false)
                    {


                        try
                        {
                            // RUC ------------------------------------------------------------------------------------------------------------------------------
                            IWebElement iRuc = driver.FindElement(By.Id("numRuc"));
                            iRuc.SendKeys(ruc);

                            Thread.Sleep(1000);

                            // TIPO DE COMPROBANTE --------------------------------------------------------------------------------------------------------------
                            IWebElement selectElement = driver.FindElement(By.XPath("/html/body/div[2]/div/div/form/div/div[3]/div[3]/select"));
                            var selectObj = new SelectElement(selectElement);

                            selectObj.SelectByValue(tip_CDP);

                            Thread.Sleep(1000);

                            // SERIE DE CDP ----------------------------------------------------------------------------------------------------------------------
                            IWebElement iSerie_CDP = driver.FindElement(By.XPath("/html/body/div[2]/div/div/form/div/div[4]/div[3]/table/tbody/tr/td[1]/input"));
                            iSerie_CDP.SendKeys(serie_CDP);

                            Thread.Sleep(1000);


                            ERRORmSG = driver.FindElement(By.Id("errorMsg")).Text;

                            // NUMERO DE CDP ---------------------------------------------------------------------------------------------------------------------
                            IWebElement iNum_CDP = driver.FindElement(By.XPath("/html/body/div[2]/div/div/form/div/div[4]/div[3]/table/tbody/tr/td[3]/input"));
                            iNum_CDP.SendKeys(num_CDP);

                            ERRORmSG = driver.FindElement(By.Id("errorMsg")).Text == ERRORmSG ? ERRORmSG : ERRORmSG + "-" + driver.FindElement(By.Id("errorMsg")).Text;


                            Thread.Sleep(1000);

                            // FECHA DE CDP ----------------------------------------------------------------------------------------------------------------------
                            IWebElement iFe_CDP = driver.FindElement(By.XPath("/html/body/div[2]/div/div/form/div/div[6]/div[3]/div/input"));
                            iFe_CDP.SendKeys(fe_CDP);

                            Thread.Sleep(1000);

                            // IMPORTE TOTAL ---------------------------------------------------------------------------------------------------------------------
                            IWebElement iImp_tot_CDP = driver.FindElement(By.XPath("/html/body/div[2]/div/div/form/div/div[7]/div[3]/input"));
                            iImp_tot_CDP.SendKeys(imp_tot_CDP);

                            Thread.Sleep(1000);

                            // BOTON CONSULTAR --------------------------------------------------------------------------------------------------------------------
                            IWebElement iBtnConsultar = driver.FindElement(By.XPath("/html/body/div[2]/div/div/form/div/div[10]/div[2]/center/button[1]"));
                            iBtnConsultar.Click();

                            Thread.Sleep(2500);

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error al Guardar \n" + ex.Message, "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            throw;
                        }


                        //--- Evaluando si existe el elemento para capturar el ERROR --------------------------------------------------------------


                        elementError = IsElementPresent(driver, By.XPath("/html/body/div[2]/div/div/form/div/div[9]/div[2]/div/div/span"));

                        bool panelResultHidden = IsElementPresent(driver, By.ClassName("panel panel-primary hidden"));


                        escritutaCorrecta = true;

                        if (panelResultHidden == true && elementError == true)
                        {

                            IWebElement iMsg = driver.FindElement(By.XPath("/html/body/div[2]/div/div/form/div/div[9]/div[2]/div/div/span"));
                            MsgError = iMsg.Text;

                            numerointentos += 1;

                            escritutaCorrecta = false;


                        }
                        //if (panelResultHidden == true && elementError == false)
                        //{
                        //    MsgError = "Error Desconocido";
                        //    escritutaCorrecta = false;

                        //}
                        else { MsgError = "OK"; }


                    }

                    //Declaramos los estados del resultado para capturar luego



                    IWebElement iEstado1 = driver.FindElement(By.Id("resEstado"));

                    estado1 = iEstado1.Text;

                    IWebElement iEstado2 = driver.FindElement(By.Id("resEstadoRuc"));

                    estado2 = iEstado2.Text;

                    IWebElement iEstado3 = driver.FindElement(By.Id("resCondicion"));

                    estado3 = iEstado3.Text;

                    resEstados = estado1 + "|" + estado2 + "|" + estado3;


                    //----- Path ------------------------------------------

                    string currentDirectory = Environment.CurrentDirectory;
                    string path = currentDirectory + @"\CDP-ESTADOS\";


                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);


                    string fileTxt = path + Path.GetFileNameWithoutExtension(fi.Name) + "_Resultado.txt";

                    //------------------------------------------------------

                    // Concatetamos los atributos de la consulta, los Estados, los mensajes y la fecha de consulta ----------------------------------

                    string Date = DateTime.Now.ToString();

                    StreamWriter sw1 = new StreamWriter(fileTxt, true);

                    String row1 = ruc + "|" + tip_CDP + "|" + serie_CDP + "|" + num_CDP + "|" + fe_CDP + "|" + imp_tot_CDP + "|" + resEstados + "|" + MsgError + "|" + Date + "|" + ERRORmSG;


                    sw1.WriteLine(row1);
                    sw1.Close();

                    txtPath.Text = fileTxt;


                    #region Clear

                    // -- Cerramos el mensaje de error de la pagina si existe

                    if (elementError == true)
                    {

                        IWebElement iDeleteMessage = driver.FindElement(By.XPath("/html/body/div[2]/div/div/form/div/div[9]/div[2]/div/div/a"));
                        iDeleteMessage.Click();
                    }

                    // -- Limpiamos los controles 
                    ruc = "";
                    tip_CDP = "";
                    serie_CDP = "";
                    num_CDP = "";
                    fe_CDP = "";
                    imp_tot_CDP = "";

                    Thread.Sleep(2000);

                    IWebElement iBtnLimpiar = driver.FindElement(By.XPath("/html/body/div[2]/div/div/form/div/div[10]/div[2]/center/button[2]"));
                    iBtnLimpiar.Click();

                    #endregion

                    Thread.Sleep(1500);

                    i++;

                    linea = read.ReadLine();

                }

                #endregion

                numFiles++;

                numRow += i + " | ";

                path_CDP_txt = "";

            }

            lblRow.Text = numRow;
            txtFiles.Text = "" + numFiles;

            #endregion

        }

        public bool IsElementPresent(IWebDriver driver, By by)
        {
            try
            {
                driver.FindElement(by);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }


    }
}
