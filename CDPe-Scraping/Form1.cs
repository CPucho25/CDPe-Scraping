using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Threading;

namespace CDPe_Scraping
{
    public partial class Form1 : Form
    {
        private static IWebDriver driver;
        static string path_Entrada_Split = Environment.CurrentDirectory + @"\1-CDPs_Input_Split\";
        static string path_Salida_Split = Environment.CurrentDirectory + @"\2-CDPs_Ouput_Split\";
        static string path_DWL = Environment.CurrentDirectory + @"\3-CDPs-Downloads\";
        static string path_SalidaJoin = Environment.CurrentDirectory + @"\4-CDPs-Ouput_Join\";

        public Form1()
        {
            InitializeComponent();

            // Path - SPLIT

            string path_Entrada_Split = Environment.CurrentDirectory + @"\1-CDPs_Input_Split\";

            if (!Directory.Exists(path_Entrada_Split))
            {
                Directory.CreateDirectory(path_Entrada_Split);
            }

            string path_Salida_Split = Environment.CurrentDirectory + @"\2-CDPs_Ouput_Split\";

            if (!Directory.Exists(path_Salida_Split))
            {
                Directory.CreateDirectory(path_Salida_Split);
            }

            // Path de Descarga

            string path_DWL = Environment.CurrentDirectory + @"\3-CDPs-Downloads\";

            if (!Directory.Exists(path_DWL))
            {
                Directory.CreateDirectory(path_DWL);
            }

            // Path - JOIN

            string path_SalidaJoin = Environment.CurrentDirectory + @"\4-CDPs-Ouput_Join\";

            if (!Directory.Exists(path_SalidaJoin))
            {
                Directory.CreateDirectory(path_SalidaJoin);
            }

            //O T H E R S
            string pathDownload = path_DWL;

            var options = new ChromeOptions();
            options.AddUserProfilePreference("profile.default_content_setting_values.automatic_downloads", 1);
            options.AddUserProfilePreference("download.default_directory", pathDownload);

            driver = new ChromeDriver(options);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            button1.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // 1) D I V I D I R    F I L E S   DE  100  EN  100  LINEAS   (SPLIT)

            DirectoryInfo di01 = new DirectoryInfo(path_Entrada_Split);

            foreach (var fi01 in di01.GetFiles())
            {
                SplitFile(path_Entrada_Split + fi01.Name, path_Salida_Split);
            }

            Thread.Sleep(4000);


            // 2) SCRAPING  S U N A T 

            String DateInicio = DateTime.Now.ToString();

            lblInicio.Text = DateInicio;
            bool elementExist = true;
            bool frameExist = true;
            bool btnDownload = true;

            string url = @"https://api-seguridad.sunat.gob.pe/v1/clientessol/4f3b88b3-d9d6-402a-b85d-6a0bc857746a/oauth2/loginMenuSol?originalUrl=https://e-menu.sunat.gob.pe/cl-ti-itmenu/AutenticaMenuInternet.htm&state=rO0ABXNyABFqYXZhLnV0aWwuSGFzaE1hcAUH2sHDFmDRAwACRgAKbG9hZEZhY3RvckkACXRocmVzaG9sZHhwP0AAAAAAAAx3CAAAABAAAAADdAAEZXhlY3B0AAZwYXJhbXN0AEsqJiomL2NsLXRpLWl0bWVudS9NZW51SW50ZXJuZXQuaHRtJmI2NGQyNmE4YjVhZjA5MTkyM2IyM2I2NDA3YTFjMWRiNDFlNzMzYTZ0AANleGVweA==";

            driver.Url = url;

            Thread.Sleep(2000);

            string ruc = txtRuc.Text;
            string user = txtUser.Text;
            string pass = txtPass.Text;
            string errorFiles = "";

            // INICIAR SESION - SUNAT
            try
            {
                IWebElement iRuc = driver.FindElement(By.Id("txtRuc"));
                iRuc.SendKeys(ruc);
                Thread.Sleep(1000);

                IWebElement iUsuario = driver.FindElement(By.Id("txtUsuario"));
                iUsuario.SendKeys(user);
                Thread.Sleep(1000);

                IWebElement iContraseña = driver.FindElement(By.Id("txtContrasena"));
                iContraseña.SendKeys(pass);

                Thread.Sleep(1000);

                IWebElement iBtnLogin = driver.FindElement(By.Id("btnAceptar"));
                iBtnLogin.Click();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al Guardar \n" + ex.Message, "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }

            Thread.Sleep(2000);

            // DOWNLOAD FILES - CDP - SUNAT
            /*try
            {*/
                IWebElement iBuscar = driver.FindElement(By.Id("txtBusca"));
                iBuscar.SendKeys("Consulta Integrada de Validez de los Comprobantes de Pago");

                Thread.Sleep(2500);

                elementExist = IsElementPresent(driver, By.XPath("/html/body/div[5]/div[2]/div[2]/div/div[1]/div/div/ul/li[2]/li[2]/li[2]/li[1]/span/span"));

                if (elementExist == false)
                {
                    iBuscar.Clear();
                    iBuscar.SendKeys("integrada");

                    Thread.Sleep(1000);

                    IWebElement iLinkCDP = driver.FindElement(By.XPath("/html/body/div[5]/div[2]/div[2]/div/div[1]/div/div/ul/li[4]/li[8]/li[18]/li[1]/span[1]/span"));
                    iLinkCDP.Click();
                }
                else {
                    IWebElement iLinkCDP = driver.FindElement(By.XPath("/html/body/div[5]/div[2]/div[2]/div/div[1]/div/div/ul/li[2]/li[2]/li[2]/li[1]/span/span"));
                    iLinkCDP.Click();
                }

                Thread.Sleep(4000);

                //Seleccionamos el frame(ya que hay un html dentro de otro html)
                IWebElement iFrameDetails1 = driver.FindElement(By.XPath("/html/body/div[4]/div/div[2]/iframe"));
                driver.SwitchTo().Frame(iFrameDetails1);

                IWebElement iBtnMasiva = driver.FindElement(By.XPath("/html/body/div[2]/div[1]/div/a[2]"));
                iBtnMasiva.Click();

                driver.SwitchTo().DefaultContent();

                Thread.Sleep(3000);

                //   PAGE 1  -------------------------------------------------------------------------------------------------------------
                IWebElement iFrameDetails2 = driver.FindElement(By.XPath("/html/body/div[4]/div/div[2]/iframe"));
                driver.SwitchTo().Frame(iFrameDetails2);

                // path

                string path = Environment.CurrentDirectory + @"\2-CDPs_Ouput_Split\";

                if (!Directory.Exists(path)) {
                    Directory.CreateDirectory(path);
                }

                DirectoryInfo di = new DirectoryInfo(path);

                // Capturamos la ruta del archivo uno x uno para la consulta masiva
                foreach (var fi in di.GetFiles())
                {
                    //Capturamos(path + fi.Name) y luego Cargamos la ruta del archivo en el elemento de la web
                    string fullName = path + fi.Name;

                    IWebElement iBtnFile = driver.FindElement(By.Id("txtarchivo"));
                    iBtnFile.SendKeys(fullName);

                    Thread.Sleep(2000);

                    //Boton GENERAR REPORTE (la ruta del archivo)
                    IWebElement iBtnEnviar = driver.FindElement(By.Id("btnEnviar"));
                    iBtnEnviar.Click();

                    Thread.Sleep(10000);
                    // Carga la pagina para mostrar el resultado de la consulta...(se puso 10 segundos de espera)

                    //   PAGE 2  -------------------------------------------------------------------------------------------------------------
                    //Seleccionamos el frame(ya que hay un html dentro de otro html)
                    driver.SwitchTo().DefaultContent();

                    elementExist = IsElementPresent(driver, By.XPath("/html/body/div[4]/div/div[2]/iframe"));

                    if (elementExist == false) {
                        while (elementExist == false)
                        {
                            Thread.Sleep(2000);
                            elementExist = IsElementPresent(driver, By.XPath("/html/body/div[4]/div/div[2]/iframe"));
                        }
                    }

                    IWebElement iFrameDetails3 = driver.FindElement(By.XPath("/html/body/div[4]/div/div[2]/iframe"));
                    driver.SwitchTo().Frame(iFrameDetails3);


                    #region Evalua si hay archivos con errores (en la pagina 1)

                    // Anuncio de error 
                    bool elementExistError = IsElementPresent(driver, By.XPath("/html/body/div[2]/div[2]/div[2]/div[2]/form/div[3]/div[1]/div/div/span"));

                    if (elementExistError == true)
                    {
                        //Capturamos el mensaje del error
                        string errorMsg = driver.FindElement(By.XPath("/html/body/div[2]/div[2]/div[2]/div[2]/form/div[3]/div[1]/div/div/span")).Text;
                        errorFiles += fi.Name +"|" + errorMsg + "\r\n";

                        if (errorMsg != "El archivo contiene lineas repetidas" && errorMsg != "El archivo contiene lineas repetidas") 
                        {
                            //descargar archivo error si el anuncion es: "Se encontraron inconsistencias, para ver el detalle dar clic "
                            driver.FindElement(By.XPath("/html/body/div[2]/div[2]/div[2]/div[2]/form/div[3]/div[1]/div/div/span/a")).Click();
                        }

                        Thread.Sleep(1000);

                        //Cerrar anuncio de error "x"
                        driver.FindElement(By.XPath("/html/body/div[2]/div[2]/div[2]/div[2]/form/div[3]/div[1]/div/div/a")).Click();

                        Thread.Sleep(1000);

                        //Boton CANCELAR (para limpiar la ruta de archivo)
                        IWebElement iBtnCancelar1 = driver.FindElement(By.Id("btnCancelar"));
                        iBtnCancelar1.Click();
                    }
                    else {

                    driver.SwitchTo().DefaultContent();

                    Thread.Sleep(2000);

                    IWebElement iFrameDetails33 = driver.FindElement(By.XPath("/html/body/div[4]/div/div[2]/iframe"));
                    driver.SwitchTo().Frame(iFrameDetails33);

                    #region Boton Descargar
                    // 1.- Evalua si existe
                    btnDownload = IsElementPresent(driver, By.XPath("/html/body/div[2]/div[2]/div[2]/div[3]/div[5]/div/div/div/div[1]/a[1]"));

                        if (btnDownload == false)
                        {
                            while (btnDownload == false)
                            {
                                Thread.Sleep(2000);
                                btnDownload = IsElementPresent(driver, By.XPath("/html/body/div[2]/div[2]/div[2]/div[3]/div[5]/div/div/div/div[1]/a[1]"));
                            }
                        }

                        Thread.Sleep(2000);

                        // 2.- Selecciona el Boton
                        IWebElement iBtnDescargar = driver.FindElement(By.XPath("/html/body/div[2]/div[2]/div[2]/div[3]/div[5]/div/div/div/div[1]/a[1]"));
                        iBtnDescargar.Click();

                        Thread.Sleep(2000);
                        #endregion

                        //Boton VOLVER (para salir del resultado)
                        IWebElement iBtnVolver = driver.FindElement(By.XPath("/html/body/div[2]/div[2]/div[2]/div[3]/div[5]/div/div/div/div[1]/a[2]"));
                        iBtnVolver.Click();

                        Thread.Sleep(1000);

                        //   PAGE 1  -------------------------------------------------------------------------------------------------------------
                        //Posicionamos de nuevo al Frame
                        driver.SwitchTo().DefaultContent();
                        //1.- Evaluamos
                        frameExist = IsElementPresent(driver, By.XPath("/html/body/div[4]/div/div[2]/iframe"));

                        if (frameExist == false)
                        {
                            while (frameExist == false)
                            {
                                Thread.Sleep(2000);
                                frameExist = IsElementPresent(driver, By.XPath("/html/body/div[4]/div/div[2]/iframe"));
                            }
                        }
                        //2.- Seleccionamos
                        IWebElement iFrameDetails4 = driver.FindElement(By.XPath("/html/body/div[4]/div/div[2]/iframe"));
                        driver.SwitchTo().Frame(iFrameDetails4);

                        //Boton CANCELAR (para limpiar la ruta de archivo)
                        IWebElement iBtnCancelar2 = driver.FindElement(By.Id("btnCancelar"));
                        iBtnCancelar2.Click();
                    }

                    #endregion

                    Thread.Sleep(1500);
                }
            /*}
            catch (Exception ex)
            {
                MessageBox.Show("Error al Guardar \n" + ex.Message, "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }*/

            Thread.Sleep(2000);

            string pathDL2 = Environment.CurrentDirectory + @"\3-CDPs-Downloads\";

            // 3) U N I R    F I L E S   (JOIN) --------------------------------------------------------------------------

            DirectoryInfo di33 = new DirectoryInfo(path_Entrada_Split);
            int count = 0;

            foreach (var fi3 in di33.GetFiles())
            {
                string nameFile = Path.GetFileNameWithoutExtension(fi3.Name);

                JoinFile(path_DWL, nameFile, path_SalidaJoin);

                count++;
            }

            txtPath.Text = pathDL2;

            Thread.Sleep(1000);

            // 4) V A C I A R  C A R P E T A S  (1, 2 y 3)

            /*vaciarCarpetas1_2_3(count);*/

            // Grabar Errores en txt(solo si existe) sino Vaciar Carpertas(significa que el proceso fue correcto)
            if (errorFiles != "") {
                File.WriteAllText(path_SalidaJoin+"error_Files.txt", errorFiles);
            } else {
                vaciarCarpetas1_2_3(count);
            }

            String DateFin = DateTime.Now.ToString();
            lblFin.Text = DateFin;
        }

        //  B O T O N E S    Y    E V E N T O S ---------------------------------------------------------
        private void btnSalir_Click(object sender, EventArgs e)
        {
            driver.Close();
            this.Close();
        }

        private void txtRuc_Enter(object sender, EventArgs e)
        {
            activarBtn();
        }
        private void txtRuc_KeyPress(object sender, KeyPressEventArgs e)
        {
            activarBtn();
        }
        private void txtPass_KeyUp(object sender, KeyEventArgs e)
        {
            activarBtn();
        }

        // P R O C E D I M I E N T O S ------------------------------------------------------------------
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

        private void SplitFile(string ruta_txt, string nueva_ruta_txt)
        {

            string[] lines = System.IO.File.ReadAllLines(ruta_txt);//16 mil

            int numFile = 0;
            string strNumFile = "";


            int cont = 0;
            string contenido = "";
            int paqueCien = lines.Length / 100;
            int sueltos = lines.Length - paqueCien * 100;

            foreach (string line in lines)
            {
                strNumFile = (numFile + 1).ToString();

                if (strNumFile.Length == 1)
                {
                    strNumFile = "00" + strNumFile;
                }
                else if (strNumFile.Length == 2)
                {
                    strNumFile = "0" + strNumFile;
                }

                if (cont < 99) { contenido += line + "\r\n"; }

                else { contenido += line; }

                if (cont == 99)
                {
                    File.WriteAllText(nueva_ruta_txt + strNumFile + ".txt", contenido);
                    contenido = "";
                    cont = -1;
                    numFile += 1;
                }
                cont++;
            }

            contenido = "";

            for (int i = 0; i < sueltos; i++)
            {
                if (i == sueltos - 1)
                {
                    strNumFile = (numFile + 1).ToString();

                    if (strNumFile.Length == 1)
                    {
                        strNumFile = "00" + strNumFile;
                    }
                    else if (strNumFile.Length == 2)
                    {
                        strNumFile = "0" + strNumFile;
                    }

                    contenido += lines[i + paqueCien * 100];

                    File.WriteAllText(nueva_ruta_txt + strNumFile + ".txt", contenido);
                }
                else {
                    contenido += lines[i + paqueCien * 100] + "\r\n";
                }
            }
        }

        private void JoinFile(string inputFile, string newNameFile, string ouputFile)
        {

            string inputPath = inputFile + @"\";
            string ouputFullName = ouputFile + @"\" + newNameFile + "_Resultado" + ".txt";

            DirectoryInfo di = new DirectoryInfo(inputPath);

            foreach (var fi in di.GetFiles())
            {
                string name = fi.Name;

                StreamReader sr1 = new StreamReader(inputPath + name);

                string linea = sr1.ReadLine();

                while (linea != null)
                {
                    StreamWriter sw1 = new StreamWriter(ouputFullName, true);

                    sw1.WriteLine(linea);
                    sw1.Close();

                    linea = sr1.ReadLine();
                }
                sr1.Close();
            }
        }

        public void vaciarCarpetas1_2_3(int cantidadFiles)
        {
            int tiempoEspera = cantidadFiles * 1500;

            Thread.Sleep(tiempoEspera);

            DirectoryInfo di1 = new DirectoryInfo(path_Entrada_Split);

            foreach (var fi1 in di1.GetFiles())
            {
                File.Delete(path_Entrada_Split + fi1.Name);
            }

            DirectoryInfo di2 = new DirectoryInfo(path_Salida_Split);

            foreach (var fi2 in di2.GetFiles())
            {
                File.Delete(path_Salida_Split + fi2.Name);
            }

            Thread.Sleep(tiempoEspera);

            DirectoryInfo di3 = new DirectoryInfo(path_DWL);

            foreach (var fi3 in di3.GetFiles())
            {
                File.Delete(path_DWL + fi3.Name);
            }
        }

        private void activarBtn()
        {
            string tRuc = txtRuc.Text;
            string tUser = txtUser.Text;
            string tPass = txtPass.Text;

            if (tRuc != "" && tUser != "" && tPass != "")
            {
                button1.Enabled = true;
                button1.BackColor = Color.LightBlue;
            }
            else
            {
                button1.Enabled = false;
                button1.BackColor = System.Drawing.SystemColors.Control;
                button1.UseVisualStyleBackColor = true;
            }
        }
    }
}
