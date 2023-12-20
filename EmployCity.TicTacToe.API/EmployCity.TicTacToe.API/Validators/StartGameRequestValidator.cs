using EmployCity.TicTacToe.API.Wrappers;
using FluentValidation;

namespace EmployCity.TicTacToe.API.Validators
{
    public class StartGameRequestValidator : AbstractValidator<StartGameRequest>
    {
        public StartGameRequestValidator()
        {
            RuleFor(x => x.StartingPlayer)
                .NotNull()
                .Must(x => BeValidMove(x))
                .WithMessage("Invalid move. It should be 'X', 'x', 'O', or 'o'.");
        }

        private static bool BeValidMove(string startingPlayer)
        {
            return startingPlayer.Equals("X", StringComparison.InvariantCultureIgnoreCase)
                || startingPlayer.Equals("O", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
