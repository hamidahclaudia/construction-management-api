using ConstructionManagement.Application.Interfaces.Repositories;
using ConstructionManagement.Domain.Entities;
using Nest;

namespace ConstructionManagement.Infrastructure.ElasticSearch;

public class ElasticSearchRepository(IElasticClient client) : IElasticSearchRepository
{
    private readonly string _indexName = "construction-projects"; 
    public async Task<IEnumerable<Project>> GetProjectsAsync()
    {
        var response = await client.SearchAsync<Project>(s => s
            .Index(_indexName) 
            .Query(q => q.MatchAll()));

        return response.IsValid ? response.Documents : Enumerable.Empty<Project>();
    }
    
    public async Task<Project?> GetProjectByIdAsync(int id)
    {
        var response = await client.GetAsync<Project>(id, g => g.Index(_indexName)); 
        return response.Found ? response.Source : null;
    }
}