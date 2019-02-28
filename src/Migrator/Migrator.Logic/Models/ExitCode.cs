namespace Migrator.Logic.Models
{
    public static class ExitCode
    {
        public const int Success = 0;

        public const int Exception = 1;

        public const int CanNotFindFile = 2;

        public const int InvalidArguments = 3;

        public const int InvalidCommand = 4;

        public const int CanNotFindDbContext = 5;
    }
}
