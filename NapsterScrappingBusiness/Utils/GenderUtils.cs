using Microsoft.Extensions.Configuration;
using NapsterScrappingBusiness.Dtos;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NapsterScrappingBusiness.Utils
{
    public static class GenderUtils
    {
        public static void GoUrlLogin(IWebDriver driver, IConfiguration _config, string parameter)
        {
            driver.Navigate().GoToUrl(_config[parameter]);      // Ingresamos a la vista de Login
        }
        public static void Login(IWebDriver driver, IConfiguration _config)
        {
            // Nos ubicamos en el texto para el login de la web
            var user = driver.FindElement(By.Name("username"));
            var pass = driver.FindElement(By.Name("password"));
            var login = driver.FindElement(By.ClassName("cbWaac"));

            // Nos logueamos con nuestras credenciales
            user.SendKeys(_config["Gender:User"]);
            pass.SendKeys(_config["Gender:Password"]);

            // Evento click
            login.Click();
        }
        public static List<GenderSubGenreDto> GenderDetails(IWebDriver driver)
        {
            List<GenderSubGenreDto> genderList = new List<GenderSubGenreDto>();
            int i = 0;
            // capturar todos los géneros
            // Mientras exista elementos tanto para Most ó Less buscará su género y subgénero
            while ((driver.FindElements(By.XPath($"//a[@data-testid='most-listened-item-{i}']")).Count > 0) || (driver.FindElements(By.XPath($"//div[@data-testid='less-listened-item-{i}']")).Count > 0))
            {
                // Validamos existencia de most
                if (driver.FindElements(By.XPath($"//a[@data-testid='most-listened-item-{i}']")).Count > 0)
                {
                    var rowsMost = driver.FindElements(By.XPath($"//a[@data-testid='most-listened-item-{i}']"));
                    // Buscamos nombre del género Most
                    string nameGender = GetNameGenderMost(rowsMost);
                    // Buscamos los subgéneros
                    var subgenre = GetSubGenreMost(driver, i);
                    // Guardamos
                    genderList.Add(new GenderSubGenreDto() { Gender = nameGender, SubGenre = subgenre });
                }

                // Validamos existencia de Less
                if (driver.FindElements(By.XPath($"//div[@data-testid='less-listened-item-{i}']")).Count > 0)
                {
                    var rowsLess = driver.FindElement(By.XPath($"//div[@data-testid='less-listened-item-{i}']"));
                    // Buscamos género y subgénero
                    var subgenres = GetSubGenreLess(driver, i);
                    foreach (var subgenre in subgenres)
                    {
                        genderList.Add(subgenre);
                    }

                }

                i++;
            }
            return genderList;
        }
        public static List<SubGenreDetail> GetSubGenreMost(IWebDriver driver, int i)
        {
            List<SubGenreDetail> subgenreList = new List<SubGenreDetail>();
            // Ubica la etiqueta e ingresa al subgénero
            var sub = driver.FindElement(By.XPath($"//a[@data-testid='most-listened-item-{i}']"));
            sub.Click();

            // Esperamos que encuentre la etiqueta que tiene la información
            WebDriverWait w = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
            w.Until(e => e.FindElements(By.XPath($"//a[@class='sc-fubCzh fsCIlM']")));

            // Lista de subgéneros
            var subgeners = driver.FindElements(By.XPath($"//h2[@class='sc-fodVek iuFLbU']"));

            // Lógica si no encuentra los subgéneros lo buscará nuevamente en 5 segundos
            // Despues de 3 intentos retorna con lo que encontró evitando un loop infinito
            int y = 0;
            while (subgeners.Count < 4)
            {
                Thread.Sleep(5000);
                subgeners = driver.FindElements(By.XPath($"//h2[@class='sc-fodVek iuFLbU']"));
                if (y > 3)
                    break;
            }

            // Para encontrar el nombre del subgénero
            var element = driver.FindElements(By.XPath($"//div[@class='sc-gkdBiK SYFvO']"));
            string[] names = new string[element.Count];
            var subgenreName = driver.FindElement(By.XPath($"//div[@class='sc-gkdBiK SYFvO']"));
            var namedetails = subgenreName.FindElements(By.XPath($"//h2[@class='sc-fodVek iuFLbU']"));
            int x = 0;
            foreach (var namedetail in namedetails)
            {
                names[x] = namedetail.Text.ToString();
                x++;
            }
            // Detalle de Subgénero
            var detailBySong = GetDetailbySong(driver);
            var detailByArtist = GetDetailbyArtist(driver);
            if(namedetails.Count == 4)
            {
                subgenreList = GenerateSubGenreDetail(names, detailBySong, detailByArtist);
            }
            if(namedetails.Count == 5)
            {
                var detailByRecommend = GetDetailbyRecommend(driver);
                subgenreList = GenerateSubGenreDetail(names, detailBySong, detailByArtist, detailByRecommend);
            }

            // Regresamos a la vista anterior
            driver.Navigate().Back();

            return subgenreList;
        }
        public static List<GenderSubGenreDto> GetSubGenreLess(IWebDriver driver, int i)
        {
            List<GenderSubGenreDto> genderSubGenreDto = new List<GenderSubGenreDto>();

            var rowsLess = driver.FindElement(By.XPath($"//div[@data-testid='less-listened-item-{i}']"));
            // Obtenemos la lista de etiquetas que tienen dentro los géneros y subgéneros
            var genderSubgenreList = rowsLess.FindElements(By.ClassName("sc-fubCzh"));
            // inicializador para recorrer la lista de genero y subgenero para obtener el detalle de cada uno
            int x = 0;
            // recorremos la lista
            foreach (var genderSubgenre in genderSubgenreList)
            {
                List<SubGenreDetail> subgenreList = new List<SubGenreDetail>();
                // Identificamos las etiquetas
                var rowsLessInternal = driver.FindElement(By.XPath($"//div[@data-testid='less-listened-item-{i}']"));
                // Encontramos la etiqueta para ingresar a los subgéneros
                var gendersInternal = rowsLessInternal.FindElements(By.ClassName("sc-fubCzh"));
                // Obtenemos los nombres de los géneros
                var gendersList = rowsLessInternal.FindElements(By.ClassName("sc-dQoVA"));
                // Obtenemos nombre de género
                string genderName = gendersList[x].Text;
                // Ingresamos al subgénero del género "x"
                gendersInternal[x].Click();

                WebDriverWait w = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
                w.Until(e => e.FindElements(By.XPath($"//a[@class='sc-fubCzh fsCIlM']")));

                // Lista de subgéneros
                var subgeners = driver.FindElements(By.XPath($"//h2[@class='sc-fodVek iuFLbU']"));

                // Lógica si no encuentra los subgéneros lo buscará nuevamente en 5 segundos
                // Despues de 3 intentos retorna con lo que encontró evitando un loop infinito
                int y = 0;
                while (subgeners.Count < 4)
                {
                    Thread.Sleep(5000);
                    subgeners = driver.FindElements(By.XPath($"//h2[@class='sc-fodVek iuFLbU']"));
                    y++;

                    if (y > 3)
                        break;
                }
                // Si no hay subgéneros pero sí una lista de títulos
                if(subgeners.Count == 0)
                {
                    // Obtenemos el detalle de la lista de títulos sin subgénero
                    var getDetailWithOutSubGenre = GetDetailWithOutSubGenre(driver);
                    var subGenreDetail = new List<SubGenreDetail>();
                    subGenreDetail.Add(new SubGenreDetail() { SubGenreName = null, SubGenreDetails = getDetailWithOutSubGenre });
                    genderSubGenreDto.Add(new GenderSubGenreDto() { Gender = genderName, SubGenre = subGenreDetail });
                }

                string[] names = new string[subgeners.Count];
                if (subgeners.Count >= 4)
                {
                    int c = 0;
                    // Recorre la lista para obtener el nombre del subgénero
                    foreach (var item in subgeners)
                    {
                        names[c] = item.Text.ToString();
                        c++;
                    }
                    // Detalle de Subgénero
                    var detailBySong = GetDetailbySong(driver);
                    var detailByArtist = GetDetailbyArtist(driver);
                    if (subgeners.Count == 4)
                    {
                        subgenreList = GenerateSubGenreDetail(names, detailBySong, detailByArtist);
                    }
                    if (subgeners.Count == 5)
                    {
                        var detailByRecommend = GetDetailbyRecommend(driver);
                        subgenreList = GenerateSubGenreDetail(names, detailBySong, detailByArtist, detailByRecommend);
                    }
                    genderSubGenreDto.Add(new GenderSubGenreDto() { Gender = genderName, SubGenre = subgenreList });
                }

                // Retorna a la vista anterior
                driver.Navigate().Back();
                x++;
            }

            return genderSubGenreDto;
        }
        public static string GetNameGenderMost(System.Collections.ObjectModel.ReadOnlyCollection<IWebElement> rows)
        {
            string response = "";
            foreach (var row in rows)
            {
                var subLess = row.FindElement(By.ClassName("elqxUq"));
                response = subLess.Text.ToString();
            }
            return response;
        }
        public static List<string> GetDetailbySong(IWebDriver driver)
        {
            var rows = driver.FindElements(By.XPath($"//p[@class='sc-dQoVA flHDrb']"));
            List<string> resultado = new List<string>();
            foreach (var item in rows)
            {
                resultado.Add(item.Text.ToString());
            }
            return resultado;
        }
        public static List<string> GetDetailbyArtist(IWebDriver driver)
        {
            var rows = driver.FindElements(By.XPath($"//p[@class='sc-dQoVA bGrqqv']"));
            List<string> resultado = new List<string>();
            foreach (var item in rows)
            {
                resultado.Add(item.Text.ToString());
            }
            return resultado;
        }
        public static List<string> GetDetailbyRecommend(IWebDriver driver)
        {
            var rows = driver.FindElements(By.XPath($"//p[@class='sc-dQoVA icehYo']"));
            List<string> resultado = new List<string>();
            foreach (var item in rows)
            {
                resultado.Add(item.Text.ToString());
            }
            return resultado;
        }
        public static List<string> GetDetailWithOutSubGenre(IWebDriver driver)
        {
            var rows = driver.FindElements(By.XPath($"//p[@class='sc-dQoVA icehYo']"));
            List<string> resultado = new List<string>();
            foreach (var item in rows)
            {
                resultado.Add(item.Text.ToString());
            }
            return resultado;
        }
        public static List<SubGenreDetail> GenerateSubGenreDetail(string[] names, List<string> songs, List<string> artists, List<string> recommends = null)
        {
            List<SubGenreDetail> resultado = new List<SubGenreDetail>();
            
            string[] songs0 = new string[20];
            songs.CopyTo(0, songs0, 0, 20);

            string[] songs2 = new string[20];
            songs.CopyTo(20, songs2, 0, 20);

            string[] songs3 = new string[12];
            songs.CopyTo(40, songs3, 0, 12);

            if (recommends == null)
            {
                resultado.Add(new SubGenreDetail() { SubGenreName = names[0], SubGenreDetails = songs0.ToList() });
                resultado.Add(new SubGenreDetail() { SubGenreName = names[1], SubGenreDetails = artists });
                resultado.Add(new SubGenreDetail() { SubGenreName = names[2], SubGenreDetails = songs2.ToList() });
                resultado.Add(new SubGenreDetail() { SubGenreName = names[3], SubGenreDetails = songs3.ToList() });
            }
            else
            {
                resultado.Add(new SubGenreDetail() { SubGenreName = names[0], SubGenreDetails = recommends });
                resultado.Add(new SubGenreDetail() { SubGenreName = names[1], SubGenreDetails = songs0.ToList() });
                resultado.Add(new SubGenreDetail() { SubGenreName = names[2], SubGenreDetails = artists });
                resultado.Add(new SubGenreDetail() { SubGenreName = names[3], SubGenreDetails = songs2.ToList() });
                resultado.Add(new SubGenreDetail() { SubGenreName = names[4], SubGenreDetails = songs3.ToList() });
            }
            return resultado;
        }
        public static List<ArtistByGendersDto> GetArtistByGender(IWebDriver driver, string name)
        { 
            List<ArtistByGendersDto> genderList = new List<ArtistByGendersDto>();
            int i = 0;
            while ((driver.FindElements(By.XPath($"//a[@data-testid='most-listened-item-{i}']")).Count > 0) || (driver.FindElements(By.XPath($"//div[@data-testid='less-listened-item-{i}']")).Count > 0))
            {
                // Validamos existencia de most
                if (driver.FindElements(By.XPath($"//a[@data-testid='most-listened-item-{i}']")).Count > 0)
                {
                    var rowsMost = driver.FindElements(By.XPath($"//a[@data-testid='most-listened-item-{i}']"));
                    // Buscamos nombre del género Most
                    string nameGender = GetNameGenderMost(rowsMost);
                    var sub = driver.FindElement(By.XPath($"//a[@data-testid='most-listened-item-{i}']"));
                    sub.Click();
                    var artists = driver.FindElement(By.XPath($"//div[@data-testid='popular-artists-section-rail']"));
                    var artistclick = artists.FindElement(By.XPath($"//a[@class='sc-fubCzh fsCIlM']"));
                    artistclick.Click();


                }
            }

            return genderList;
        }
    }
}
