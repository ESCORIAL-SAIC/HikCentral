using HikCentral.Dispositivos.Models;
using HikCentral.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var serviceCollection = new ServiceCollection();

Configuration.Load();

serviceCollection.AddDbContext<ESCORIALContext>(options =>
    options.UseNpgsql(Configuration.ConnectionString.Escorial));

var serviceProvider = serviceCollection.BuildServiceProvider();
using var context = serviceProvider.GetRequiredService<ESCORIALContext>();

var dispositivos = await HikCentral.Controllers.AccessLevelAndAccessGroup.AccessLevelListController.Get(1, 500, 1);

var itemsExistentesQuery = context.Itemtipoclasificadors
    .Where(i =>
        i.BoPlaceId == new Guid("CD2B247F-AA63-484E-B6C8-97BC3DC0F914") &&
        i.Activestatus != 2)
    .OrderBy(i => i.Codigo);
foreach (var dispositivo in dispositivos.data.list)
{
    var itemsExistentes = await itemsExistentesQuery.ToListAsync();
    if (itemsExistentes.Exists(i => i.Codigo == dispositivo.privilegeGroupId))
        continue;
    var item = new Itemtipoclasificador()
    {
        Id = Guid.NewGuid(),
        BoPlaceId = new Guid("cd2b247f-aa63-484e-b6c8-97bc3dc0f914"),
        PlaceownerId = new Guid("e77e60b2-5c3f-4a8e-9533-0e1070f93e28"),
        Activestatus = 0,
        Codigo = dispositivo.privilegeGroupId,
        Nombre = dispositivo.privilegeGroupName
    };
    context.Add(item);
}

await context.SaveChangesAsync();

Console.ReadKey();