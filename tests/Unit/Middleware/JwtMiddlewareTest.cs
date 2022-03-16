using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using backend.Helpers;
using backend.Middlewares;
using backend_tests.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace backend_tests.Unit.Middleware
{
    public class JwtMiddlewareTest
    {
        public ITestOutputHelper Output { get; }

        public JwtMiddlewareTest(ITestOutputHelper output)
        {
            Output = output;
        }

        private string GetJwtTokenWithClaims(string secret, List<Claim> claims)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        [Fact]
        public async Task TestShouldAttachUserAndAccountTypeToTheContext()
        {
            var httpContextMock = new Mock<HttpContext>();
            string superSecret = "super-secret-token";
            string jwtToken = this.GetJwtTokenWithClaims(
                superSecret, 
                new List<Claim>()
                {
                    new("id", "1"),
                    new("accountType", AccountTypeEnum.Doctor.ToString()),
                }
            );

            var headers = new Dictionary<string, StringValues>()
            {
                {"Authorization", new StringValues(jwtToken)}
            };
            var contextItems = new Dictionary<object, object?>();

            httpContextMock.Setup(context => context.Request.Headers).Returns(new HeaderDictionary(headers));
            httpContextMock.Setup(x => x.Items).Returns(contextItems);

            var requestDelegate = new RequestDelegate((innerContext) => Task.FromResult(0));
            var middleware = new JwtMiddleware(requestDelegate, Options.Create(new JwtSettings() { SecretToken = superSecret}));

            var dataContextMock = DbHelper.GetMockedDataContextWithAccounts();

            var foundModel = dataContextMock.Object.Doctors.First();

            await middleware.Invoke(httpContextMock.Object, dataContextMock.Object);

            Output.WriteLine(jwtToken);

            Assert.True(contextItems.ContainsKey("User"));
            Assert.True(contextItems.ContainsKey("AccountType"));
            Assert.Equal(AccountTypeEnum.Doctor, contextItems["AccountType"]);
            Assert.Equal(foundModel, contextItems["User"]);
        }
    }
}
