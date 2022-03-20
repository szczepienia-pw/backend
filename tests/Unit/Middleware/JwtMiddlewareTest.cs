using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Helpers;
using backend.Middlewares;
using backend_tests.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Moq;
using Xunit;

namespace backend_tests.Unit.Middleware
{
    public class JwtMiddlewareTest
    {
        [Fact]
        public async Task TestShouldAttachUserAndAccountTypeToTheContext()
        {
            var httpContextMock = new Mock<HttpContext>();
            string superSecret = "super-secret-token";
            var jwtGenerator = new JwtGenerator(
                Options.Create(new JwtSettings() {SecretToken = superSecret})
            );
            string jwtToken = jwtGenerator.GenerateJwtToken(1, AccountTypeEnum.Doctor);

            var headers = new Dictionary<string, StringValues>()
            {
                {"Authorization", new StringValues(jwtToken)}
            };
            var contextItems = new Dictionary<object, object?>();

            httpContextMock.Setup(context => context.Request.Headers).Returns(new HeaderDictionary(headers));
            httpContextMock.Setup(context => context.Items).Returns(contextItems);

            var requestDelegate = new RequestDelegate((innerContext) => Task.FromResult(0));
            var middleware = new JwtMiddleware(requestDelegate, Options.Create(new JwtSettings() { SecretToken = superSecret}));

            var dataContextMock = DbHelper.GetMockedDataContextWithAccounts();

            var foundModel = dataContextMock.Object.Doctors.First();

            await middleware.Invoke(httpContextMock.Object, dataContextMock.Object);

            Assert.True(contextItems.ContainsKey("User"));
            Assert.True(contextItems.ContainsKey("AccountType"));
            Assert.Equal(AccountTypeEnum.Doctor, contextItems["AccountType"]);
            Assert.Equal(foundModel, contextItems["User"]);
        }
    }
}
