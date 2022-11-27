using MediatR;
using Microsoft.Extensions.Configuration;
using NapsterScrappingBusiness.Dtos;
using NapsterScrappingBusiness.Utils;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NapsterScrappingBusiness.Gender
{
    public class ArtistByGender
    {
        public class Query : IRequest<List<ArtistByGendersDto>>
        {
            public string Name { get; set; }
        }
        public class Handler : IRequestHandler<Query, List<ArtistByGendersDto>>
        {
            private readonly IConfiguration _config;
            public Handler(IConfiguration config)
            {
                _config = config;
            }

            public async Task<List<ArtistByGendersDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                IWebDriver driver = new ChromeDriver();
                // Vamos a la url Login 
                GenderUtils.GoUrlLogin(driver, _config, ParameterKeys.GenderLogin);

                // Nos logueamos
                GenderUtils.Login(driver, _config);

                // Maximizamos la pantalla para el scrapping completo
                driver.Manage().Window.Maximize();

                // Esperamos que la web se actualice 
                WebDriverWait w = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
                w.Until(e => e.Url == "https://web.napster.com/");

                // Damos click al botón Search para que se muestre los géneros
                var search = driver.FindElement(By.ClassName("sc-jrAFXE"));
                search.Click();

                // Esperamos que se liste los géneros, que muestre la Clase 'elqxUq'
                w.Until(ExpectedConditions.ElementIsVisible(By.ClassName("elqxUq")));

                // Obtener la lista de géneros
                List<ArtistByGendersDto> genderDetails = GenderUtils.GetArtistByGender(driver, request.Name);

                return genderDetails;
            }
        }
    }
}
