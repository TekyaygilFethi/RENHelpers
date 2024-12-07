using Bogus;
using EFCore.BulkExtensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RENHelpers.DataAccessHelpers;
using RENHelpers.DataAccessHelpers.CacheHelpers;
using RENHelpers.DataAccessHelpers.DatabaseHelpers;
using RENHelpers.ExampleProject.Data;
using RENHelpers.ExampleProject.Database;

namespace RENHelpers.ExampleProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataAccessExampleController : ControllerBase
    {
        private readonly IRENCacheService _customCacheService;
        private readonly IRENRepository<User> _userRepository;
        private readonly IRENRepository<Side> _sideRepository;
        private readonly IRENUnitOfWork<RENDbContext> _uow;

        public DataAccessExampleController(IRENUnitOfWork<RENDbContext> uow, IRENCacheService customCacheService)
        {
            // _userRepository = uow.GetMyRepository<User>();
            _customCacheService = customCacheService;
            _sideRepository = uow.GetRENRepository<Side>();
            _userRepository = uow.GetRENRepository<User>();
            _uow = uow;
        }

        [HttpGet]
        [Route("GetLightSideUsers")]
        public async Task<IActionResult> GetLightSideUsers()
        {
            var lightSideUsersList = await _userRepository.GetListAsync(isReadOnly: true);
            var lightSideUsersQueryable = await _userRepository.GetQueryableAsync(isReadOnly: true);
            var lightSideUser = await _userRepository.GetSingleAsync(filter: _ => _.Id == 1, isReadOnly: false);

            return Ok();
        }

        [HttpGet]
        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            var cacheKey = "users";
            var users = await _customCacheService.GetAsync<List<User>>(cacheKey);

            if (users != null) return Ok(users);
            users = await _userRepository.GetListAsync();
            await _customCacheService.SetAsync(cacheKey, users);

            return Ok(users);
        }

        [HttpGet]
        [Route("GetUser")]
        public async Task<IActionResult> Index([FromQuery] int Id)
        {
            var cacheKey = $"users_{Id}";
            var user = await _customCacheService.GetAsync<User>(cacheKey);
            if (user != null) return Ok(user);

            var allUsersCacheKey = "users";

            if (user != null) return Ok(user);

            user = await _userRepository.GetSingleAsync(_ => _.Id == Id);
            await _customCacheService.SetAsync(cacheKey, user);

            return Ok(user);
        }

        [HttpGet]
        [Route("GetUserByCustomFunction")]
        public async Task<IActionResult> GetUserByCustomFunction([FromQuery] int Id)
        {
            var cts = new CancellationTokenSource();
            var emptyUsers = _userRepository.GetList();
            var users = _userRepository.GetList(include: i => i.Include(_ => _.Side).ThenInclude(_ => _.Test).ThenInclude(_ => _.TestDescriptions));
            var user = await _userRepository.GetSingleAsync(_ => _.Id == Id, cancellationToken: cts.Token);
            return Ok(user);
        }

        [HttpGet]
        [Route("GetLightSideUsers2")]
        public async Task<IActionResult> GetLightSideUsers2([FromQuery] int Id)
        {
            var cts = new CancellationTokenSource();
            var emptyUsers = _userRepository.GetList();
            var users = _userRepository.GetList(include: i => i.Include(_ => _.Side).ThenInclude(_ => _.Test).ThenInclude(_ => _.TestDescriptions));
            var user = await _userRepository.GetSingleAsync(_ => _.Id == Id, cancellationToken: cts.Token);
            return Ok(user);
        }

        [HttpGet]
        [Route("GetLightSides")]
        public async Task<IActionResult> GetLightSides([FromQuery] int Id)
        {
            List<Side> lightSideList = await _sideRepository.GetListAsync(filter: _ => _.Name == "Light Side");
            IQueryable<Side> lightSideQueryable = await _sideRepository.GetQueryableAsync(filter: _ => _.Name == "Light Side");
            Side lightSide = await _sideRepository.GetSingleAsync(filter: _ => _.Name == "Light Side");

            return Ok();
        }

        [HttpGet]
        [Route("GetUsersOrdered")]
        public async Task<IActionResult> GetUsersOrdered()
        {
            List<User> orderedUsersList = await _userRepository.GetListAsync(orderBy: _ => _.OrderBy(_ => _.Name).ThenByDescending(_ => _.Id));
            IQueryable<User> orderedUserQueryable = await _userRepository.GetQueryableAsync(orderBy: _ => _.OrderBy(_ => _.Name).ThenBy(_ => _.Id));

            return Ok();
        }

        [HttpPost]
        [Route("BulkInsertUsers")]
        public async Task<IActionResult> BulkInsertUsers([FromQuery] int size, [FromQuery] int batchSize)
        {
            var sideIds = _sideRepository.GetQueryableAsync().GetAwaiter().GetResult().Select(_ => _.Id).ToList();

            var userFaker = new Faker<User>()
                .RuleFor(u => u.Name, f => f.Name.FirstName())
                .RuleFor(u => u.Surname, f => f.Name.LastName())
                .RuleFor(u => u.SideId, f => f.PickRandom(sideIds));

            var users = userFaker.Generate(size);

            var bulkConfig = new BulkConfig()
            {
                BatchSize = batchSize
            };
            var now = DateTime.Now;
            Console.WriteLine($"Process started on {now.ToString()}");
            await _userRepository.BulkInsertAsync(users, bulkConfig);
            var now2 = DateTime.Now;
            Console.WriteLine($"Process finished on {now2.ToString()}");
            Console.WriteLine($"Process elapsed time: {(now2 - now).TotalSeconds} seconds");
            return Ok();
        }

        [HttpPost]
        [Route("NormalInsertUsers")]
        public async Task<IActionResult> NormalInsertUsers([FromQuery] int size)
        {
            var sideIds = _sideRepository.GetQueryableAsync().GetAwaiter().GetResult().Select(_ => _.Id).ToList();

            var userFaker = new Faker<User>()
                .RuleFor(u => u.Name, f => f.Name.FirstName())
                .RuleFor(u => u.Surname, f => f.Name.LastName())
                .RuleFor(u => u.SideId, f => f.PickRandom(sideIds));

            var users = userFaker.Generate(size);

            var now = DateTime.Now;
            Console.WriteLine($"Process started on {now.ToString()}");
            await _userRepository.InsertAsync(users);
            await _uow.SaveChangesAsync();
            var now2 = DateTime.Now;
            Console.WriteLine($"Process finished on {now2.ToString()}");
            Console.WriteLine($"Process elapsed time: {(now2 - now).TotalSeconds} seconds");

            return Ok();
        }

        [HttpPost]
        [Route("InsertWithTransaction")]
        public async Task<IActionResult> InsertWithTransaction([FromQuery] int size)
        {
            var sideIds = await _sideRepository.GetQueryable().Select(_ => _.Id).ToListAsync();

            var userFaker = new Faker<User>()
                .RuleFor(u => u.Name, f => f.Name.FirstName())
                .RuleFor(u => u.Surname, f => f.Name.LastName())
                .RuleFor(u => u.SideId, f => f.PickRandom(sideIds));

            var users = userFaker.Generate(size);

            var now = DateTime.Now;
            Console.WriteLine($"Process started on {now.ToString()}");
            _uow.BeginTransaction();

            try
            {
                await _userRepository.InsertAsync(users);
                await _uow.SaveChangesAsync();

                var newSide = new Side { Name = "My Side" };
                await _sideRepository.InsertAsync(newSide);

                await _uow.SaveChangesAsync();

                Console.WriteLine($"Id:{newSide.Id}");

                await _uow.CommitTransactionAsync();
            }
            catch (Exception)
            {
                await _uow.RollbackTransactionAsync();
                throw;
            }

            var now2 = DateTime.Now;
            Console.WriteLine($"Process finished on {now2.ToString()}");
            Console.WriteLine($"Process elapsed time: {(now2 - now).TotalSeconds} seconds");

            return Ok();
        }
    }
}
