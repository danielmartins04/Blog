using Blog.Data;
using Blog.Extensions;
using Blog.Models;
using Blog.Services;
using Blog.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureIdentity.Password;

namespace Blog.Controllers {
    [ApiController]
    public class AccountController : ControllerBase {
        [HttpPost("v1/accounts")]
        public async Task<IActionResult> Post(
            [FromBody]RegisterViewModel model,
            [FromServices]BlogDataContext context
        ) {
            if (!ModelState.IsValid)
                return BadRequest(new ResultViewModel<string>(ModelState.GetErrors()));

            var user = new User
            {
                Name = model.Name,
                Email = model.Email,
                Slug = model.Email.Replace("@", "-").Replace(".", "-")
            };

            var password = PasswordGenerator.Generate(25);
            user.PasswordHash = PasswordHasher.Hash(password);

            try {
                await context.Users.AddAsync(user);
                await context.SaveChangesAsync();

                return Ok(new ResultViewModel<dynamic>(new {
                    user = user.Email, password
                }));
            }
            catch (DbUpdateException) {
                return StatusCode(400, new ResultViewModel<string>("Erro ao criar usuário."));
            } catch {
                return StatusCode(500, new ResultViewModel<string>("Falha interna no servidor"));
            }
        }

        [HttpPost("v1/accounts/login")]
        public async Task<IActionResult> Login(
            [FromBody]LoginViewModel model,
            [FromServices]TokenService tokenService,
            [FromServices]BlogDataContext context)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResultViewModel<string>(ModelState.GetErrors()));

            var user = await context
                .Users
                .AsNoTracking()
                .Include(x => x.Roles)
                .FirstOrDefaultAsync(x => x.Email == model.Email);

            if (user == null)
                return StatusCode(401, new ResultViewModel<string>("Usuário ou senha inválida"));

            if (!PasswordHasher.Verify(user.PasswordHash, model.Password))
                return StatusCode(401, new ResultViewModel<string>("Usuario ou senha invalida."));
            
            try {
                var token = tokenService.GenerateToken(user);
                return Ok(new ResultViewModel<string>(token, errors: null));
            } catch {
                return StatusCode(500, new ResultViewModel<string>("Falha interna ao gerar token"));
            }
        }
    }
}