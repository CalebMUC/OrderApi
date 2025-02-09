using Minimart_Api.Services.OpenSearchService;
using OpenSearch.Client;

//using OpenSearch.Net;
using System.Collections.Generic;
using System.Threading.Tasks;

public class OpenSearchService : IOpenSearchService
{
    private readonly IOpenSearchClient _client;

    public OpenSearchService(IOpenSearchClient client)
    {
        _client = client;
    }

    // Create the index if it doesn't exist
    public async Task CreateIndexAsync(string indexName)
    {
        var createIndexResponse = await _client.Indices.CreateAsync(indexName, c => c
            .Map<TProduct>(m => m
                .AutoMap()
                .Properties(p => p
                    .Text(t => t.Name(n => n.ProductName))
                    .Text(t => t.Name(n => n.Description))
                    .Number(n => n.Name(n => n.Price).Type(NumberType.Double))
                    .Keyword(k => k.Name(n => n.Category))
                    .Completion(c => c.Name(n => n.Suggest))
                )
            )
        );

        if (!createIndexResponse.IsValid)
        {
            throw new Exception("Failed to create index: " + createIndexResponse.DebugInformation);
        }
    }

    // Index a single product
    public async Task IndexProductAsync(TProduct product)
    {
        product.Suggest = new CompletionField { Input = new[] { product.SearchKeyWord } };
        var indexResponse = await _client.IndexAsync(product, i => i.Index("searchproducts").Id(product.ProductId));

        if (!indexResponse.IsValid)
        {
            throw new Exception("Failed to index product: " + indexResponse.DebugInformation);
        }
    }

    // Search for products
    public async Task<IEnumerable<TProduct>> SearchProductsAsync(string query)
    {
        var response = await _client.SearchAsync<TProduct>(s => s
            .Index("searchproducts")
            .Query(q => q
                .MultiMatch(m => m
                    .Fields(f => f
                        .Field(p => p.ProductName)
                        .Field(p => p.Description)
                        .Field(p => p.SearchKeyWord)
                    )
                    .Query(query)
                )
            )
        );

        return response.Documents;
    }

    // Autocomplete suggestions
    public async Task<IEnumerable<string>> AutocompleteAsync(string query)
    {
        var response = await _client.SearchAsync<TProduct>(s => s
            .Index("searchproducts")
            .Suggest(su => su
                .Completion("product-suggestions", c => c
                    .Field(f => f.Suggest)
                    .Prefix(query)
                    .Fuzzy(f => f.Fuzziness(Fuzziness.Auto))
                    .Size(5)
                )
            )
        );

        return response.Suggest["product-suggestions"]
            .SelectMany(s => s.Options)
            .Select(o => o.Text);
    }
}