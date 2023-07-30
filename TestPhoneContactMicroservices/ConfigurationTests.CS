using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestPhoneContactMicroservices
{
    public class ConfigurationTests
    {
        [Fact]
        public void DatabaseConnectionString_ShouldBeCorrectlyConfigured()
        {
            // Arrange
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            // Act
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            // Assert
            Assert.NotNull(connectionString);
            Assert.Contains("Host=localhost", connectionString);
            Assert.Contains("Database=ContactDirectoryDB", connectionString);
            Assert.Contains("Username=postgres", connectionString);
            Assert.Contains("Password=Bigboy95", connectionString);
        }
    }
}
