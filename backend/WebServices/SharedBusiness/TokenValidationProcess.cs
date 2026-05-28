using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using WebServices.Controllers;
using WebServices.DataAccess;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace WebServices.SharedBusiness
{
    /// <summary>
    /// Handles the validation of authorization tokens and the extraction of user information, 
    /// such as roles and usernames, from JSON Web Tokens (JWT).
    /// </summary>
    public class TokenValidationProcess
    {
        private readonly IConfiguration _config;
        private readonly DatabaseContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenValidationProcess"/> class.
        /// </summary>
        /// <param name="config">The application configuration properties, used to access JWT settings.</param>
        /// <param name="dbContext">The database context used for validation operations.</param>
        public TokenValidationProcess(IConfiguration config, DatabaseContext dbContext)
        {
            _config = config;
            _dbContext = dbContext;
        }

        /// <summary>
        /// Validates the provided authorization header, checks if the user's role is within the authorized roles, 
        /// and extracts the username directly from the JWT claims.
        /// </summary>
        /// <param name="requestHeader">The HTTP authorization header containing the Bearer token.</param>
        /// <param name="authorizedRoles">A list of roles that are permitted to access the requested resource.</param>
        /// <returns>
        /// A tuple containing:
        /// <list type="bullet">
        /// <item><description><c>tokenIsValid</c>: A boolean indicating if the token is valid.</description></item>
        /// <item><description><c>errorStatus</c>: The HTTP status code to return in case of an error (e.g., 401, 403).</description></item>
        /// <item><description><c>errorMessage</c>: A descriptive error message if validation fails; otherwise, null.</description></item>
        /// <item><description><c>doctorId</c>: The ID of the doctor associated with the token, or null if not applicable.</description></item>
        /// <item><description><c>userName</c>: The username extracted from the token claims.</description></item>
        /// </list>
        /// </returns>
        public async Task<(bool tokenIsValid, int errorStatus, string? errorMessage, int? doctorId, string userName)?> ValidateAuthorizationAsync(string requestHeader, List<string> authorizedRoles)
        {
            var authorizationHeader = requestHeader;

            // If there is no token, return string.Empty for the userName
            if (string.IsNullOrEmpty(authorizationHeader))
                return (false, StatusCodes.Status401Unauthorized, "Invalid authorization token.", null, string.Empty);

            var validation = new TokenRoleValidator(_dbContext);
            var validationResult = await validation.ValidateTokenAndGetRole(authorizationHeader, _config["Jwt:Key"], _config["Jwt:Issuer"]);

            if (!validationResult.tokenIsValid)
            {
                return (false, StatusCodes.Status401Unauthorized, validationResult.message, null, string.Empty);
            }

            if (!authorizedRoles.Contains(validationResult.role))
            {
                return (false, StatusCodes.Status403Forbidden, validationResult.role + " does not have permission to execute this process.", null, string.Empty);
            }

            // NEW LOGIC: Extract the username directly from the JWT
            string userName = "System"; // Default value in case of failure
            try
            {
                var token = authorizationHeader.Replace("Bearer ", "").Trim();
                var handler = new JwtSecurityTokenHandler();

                if (handler.CanReadToken(token))
                {
                    var jwtToken = handler.ReadJwtToken(token);

                    // The username in JWTs is usually stored in "unique_name", "name", or the standard .NET claim
                    userName = jwtToken.Claims.FirstOrDefault(c => c.Type == "Name")?.Value ?? "UnknownUser";
                }
            }
            catch
            {
                // If deserialization fails for any reason, the default value is maintained
            }

            // TODO: If the role is "Doctor", you might want to retrieve the doctorId from the database based on the token or user information. For now, we return 1 for doctorId.
            return (true, StatusCodes.Status200OK, null, 1, userName);
        }
    }
}