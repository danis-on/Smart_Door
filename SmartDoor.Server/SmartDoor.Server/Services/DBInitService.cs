using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

// ReSharper disable StringLiteralTypo

namespace SmartDoor.Server.Services
{
    public class DBInitService : IHostedService
    {
        private readonly ILogger<DBInitService> _logger;
        private readonly SqliteConnection _db;

        public DBInitService(ILogger<DBInitService> logger, SqliteConnection db)
        {
            _logger = logger;
            _db = db;
        }
        
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _db.ExecuteAsync(@"
create table if not exists user
(
    id           integer not null
        constraint user_pk
            primary key autoincrement,
    login        text    not null,
    role         int     not null,
    passwordHash text    not null,
    pinHash      text    null,
    totp         text    null
);

create unique index if not exists user_login_uindex
    on user (login);
    ").ConfigureAwait(false);

            if (1 == await _db.ExecuteAsync(@"insert into user (login, role, passwordHash)
            select 'admin', 1, 'bb1fb0facf769d730600254116a5ccce4a6c0f1756788fda142f063cd1802aa3'
            where 0 = (select count(1) from user where login = 'admin');").ConfigureAwait(false))
                _logger.LogWarning("Created default admin user with password 'password' - you should change the password as soon as possible");
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}