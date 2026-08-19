namespace Galileo.DesignSystem.Demo.Models;

public sealed class DemoProjectStore
{
    private readonly object syncRoot = new();
    private readonly Dictionary<int, DemoProject> projects = CreateProjects()
        .ToDictionary(project => project.ProjectId);

    public IReadOnlyList<DemoProject> GetAll()
    {
        lock (syncRoot)
        {
            return projects.Values
                .OrderBy(project => project.ProjectNumber, StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }
    }

    public DemoProject? Get(int projectId)
    {
        lock (syncRoot)
        {
            return projects.GetValueOrDefault(projectId);
        }
    }

    public DemoProject? UpdateName(int projectId, string name)
    {
        lock (syncRoot)
        {
            if (!projects.TryGetValue(projectId, out var project))
                return null;

            var updatedProject = project with { Name = name };
            projects[projectId] = updatedProject;
            return updatedProject;
        }
    }

    private static IReadOnlyList<DemoProject> CreateProjects()
    {
        return
        [
            new(1, "GT-2026-001", "Renovatie gemaal Kinderdijk", "Waterschap Rivierenland", "Kinderdijk", "Actief", "Groot", new DateTime(2026, 1, 12), null),
            new(2, "GT-2026-002", "Onderhoud gemaal De Esch", "Gemeente Rotterdam", "Rotterdam", "In voorbereiding", "Klein", new DateTime(2026, 2, 2), null),
            new(3, "GT-2026-003", "Vervanging besturing gemaal Westland", "Provincie Zuid-Holland", "Naaldwijk", "Actief", "Groot", new DateTime(2026, 1, 26), null),
            new(4, "GT-2026-004", "Inspectie gemaal De Wetering", "Waterschap Rivierenland", "Tiel", "Afgerond", "Klein", new DateTime(2025, 10, 6), new DateTime(2025, 12, 18)),
            new(5, "GT-2026-005", "Calamiteit gemaal Vlietpolder", "Provincie Zuid-Holland", "Leiden", "Actief", "Calamiteit", new DateTime(2026, 2, 16), null),
            new(6, "GT-2026-006", "Renovatie gemaal Zuidland", "Gemeente Rotterdam", "Zuidland", "In voorbereiding", "Groot", new DateTime(2026, 3, 9), null),
            new(7, "GT-2026-007", "Nieuwe pompinstallatie Oude Maas", "Waterschap Rivierenland", "Dordrecht", "Actief", "Groot", new DateTime(2026, 2, 23), null),
            new(8, "GT-2026-008", "Klein onderhoud gemaal Schiedam", "Gemeente Rotterdam", "Schiedam", "Afgerond", "Klein", new DateTime(2025, 11, 3), new DateTime(2026, 1, 30)),
            new(9, "GT-2026-009", "Storingsherstel gemaal Katwijk", "Provincie Zuid-Holland", "Katwijk", "Actief", "Calamiteit", new DateTime(2026, 3, 2), null),
            new(10, "GT-2026-010", "Aanpassing afvoer gemaal De Meije", "Waterschap Rivierenland", "Woerden", "In voorbereiding", "Klein", new DateTime(2026, 4, 13), null),
            new(11, "GT-2026-011", "Renovatie gemaal Brielle", "Gemeente Rotterdam", "Brielle", "Actief", "Groot", new DateTime(2026, 3, 23), null),
            new(12, "GT-2026-012", "Inspectie gemaal Reeuwijk", "Provincie Zuid-Holland", "Reeuwijk", "Afgerond", "Klein", new DateTime(2025, 12, 1), new DateTime(2026, 2, 6)),
            new(13, "GT-2026-013", "Vervanging elektrotechniek gemaal Lage Zwaluwe", "Waterschap Rivierenland", "Lage Zwaluwe", "Actief", "Groot", new DateTime(2026, 4, 6), null),
            new(14, "GT-2026-014", "Calamiteit gemaal Spaanse Polder", "Gemeente Rotterdam", "Rotterdam", "Actief", "Calamiteit", new DateTime(2026, 4, 20), null),
            new(15, "GT-2026-015", "Onderhoud gemaal Maasdijk", "Provincie Zuid-Holland", "Maasdijk", "In voorbereiding", "Klein", new DateTime(2026, 5, 4), null),
            new(16, "GT-2026-016", "Renovatie gemaal Boven-Hardinxveld", "Waterschap Rivierenland", "Hardinxveld-Giessendam", "In voorbereiding", "Groot", new DateTime(2026, 5, 11), null),
            new(17, "GT-2026-017", "Nieuwe aandrijving gemaal Hoek van Holland", "Gemeente Rotterdam", "Hoek van Holland", "Actief", "Groot", new DateTime(2026, 4, 27), null),
            new(18, "GT-2026-018", "Inspectie gemaal Rijnsburg", "Provincie Zuid-Holland", "Rijnsburg", "Afgerond", "Klein", new DateTime(2026, 1, 19), new DateTime(2026, 3, 13)),
            new(19, "GT-2026-019", "Aanleg tijdelijke pomp gemaal Culemborg", "Waterschap Rivierenland", "Culemborg", "Actief", "Calamiteit", new DateTime(2026, 5, 18), null),
            new(20, "GT-2026-020", "Optimalisatie gemaal Pernis", "Gemeente Rotterdam", "Pernis", "In voorbereiding", "Groot", new DateTime(2026, 6, 1), null),
            new(21, "GT-2026-021", "Onderhoud gemaal Zoetermeer", "Provincie Zuid-Holland", "Zoetermeer", "Actief", "Klein", new DateTime(2026, 5, 25), null),
            new(22, "GT-2026-022", "Vervanging krooshekreiniger gemaal Beesd", "Waterschap Rivierenland", "Beesd", "In voorbereiding", "Groot", new DateTime(2026, 6, 8), null),
            new(23, "GT-2026-023", "Calamiteit gemaal Hoekse Waard", "Provincie Zuid-Holland", "Oud-Beijerland", "Actief", "Calamiteit", new DateTime(2026, 6, 15), null),
            new(24, "GT-2026-024", "Renovatie gemaal Rozenburg", "Gemeente Rotterdam", "Rozenburg", "In voorbereiding", "Groot", new DateTime(2026, 6, 22), null),
            new(25, "GT-2026-025", "Energieverbetering gemaal Asperen", "Waterschap Rivierenland", "Asperen", "Actief", "Klein", new DateTime(2026, 7, 6), null),
            new(26, "GT-2026-026", "Onderhoud gemaal Ameide", "Waterschap Rivierenland", "Ameide", "In voorbereiding", "Klein", new DateTime(2026, 7, 13), null),
            new(27, "GT-2026-027", "Renovatie gemaal Leerdam", "Waterschap Rivierenland", "Leerdam", "Actief", "Groot", new DateTime(2026, 7, 20), null)
        ];
    }
}
