using ACME.DataLayer.Documents;
using ACME.DataLayer.Documents.Converters;
using ACME.DataLayer.Entities;
using Newtonsoft.Json;

namespace ACME.Backend.Tools.Converters;

public static class DocumentExtensions
{
    public static BrandDocument ToDocument(this Brand brand)
    {
        return new BrandDocument
        {
            Id = brand.Id,
            Type = DocumentType.Brand,
            Name = brand.Name,
            Website = brand.Website
        };
    }
    public static ReviewDocument ToDocument(this ConsumerReview review)
    {
        return new ReviewDocument
        {
            Id = review.Id,
            ProductId = review.ProductId.ToDocumentId(DocumentType.Product),
            PartitionKey = review.ProductId.ToDocumentId(DocumentType.Product),
            Score = review.Score,
            Text = review.Text,
            DateBought = review.DateBought,
            ReviewType = (int)review.ReviewType
        };
    }
    public static ReviewDocument ToDocument(this ExpertReview review)
    {
        return new ReviewDocument
        {
            Id = review.Id,
            ProductId = review.ProductId.ToDocumentId(DocumentType.Product),
            PartitionKey = review.ProductId.ToDocumentId(DocumentType.Product),
            Score = review.Score,
            Text = review.Text,
            Organization = review.Organization,
            ReviewType = (int)review.ReviewType
        };
    }
    public static PriceDocument ToDocument(this Price price)
    {
        return new PriceDocument
        {
            Id = price.Id,
            BasePrice = price.BasePrice,
            PriceDate = price.PriceDate,
            ProductId = price.ProductId?.ToDocumentId(DocumentType.Product),
            PartitionKey = price.ProductId?.ToDocumentId(DocumentType.Product),
            ShopName = price.ShopName
        };
    }
    public static ProductGroupDocument ToDocument(this ProductGroup group)
    {
        return new ProductGroupDocument
        {
            Id = group.Id,
            Name = group.Name,
            Image = group.Image
        };
    }
    public static ProductDocument ToDocument(this Product product)
    {
        return new ProductDocument
        {
            Id = product.Id,
            PartitionKey = product.Id.ToDocumentId(DocumentType.Product),
            Name = product.Name,
            Image = product.Image,
            BrandId = product.BrandId.ToDocumentId(DocumentType.Brand),
            ProductGroupId = product.ProductGroupId.ToDocumentId(DocumentType.ProductGroup)
        };       
    }
    public static ReviewerDocument ToDocument(this Reviewer reviewer)
    {
        return new ReviewerDocument
        {
            Id = reviewer.Id,
            Name = reviewer.Name,
            Email = reviewer.Email,
            UserName = reviewer.UserName,
            PasswordHash = reviewer.PasswordHash,
            PasswordSalt = reviewer.PasswordSalt
        };
    }
    public static SpecificationDocument ToDocument(this Specification spec, SpecificationDefinition specdef)
    {
        var specification = new SpecificationDocument
        {
            Id = spec.Id,
            Key = spec.Key,
            ProductId = spec.ProductId.ToDocumentId(DocumentType.Product),
            PartitionKey = spec.ProductId.ToDocumentId(DocumentType.Product),
            DataType = specdef.Type,
            Description = specdef.Description,
            Unit = specdef.Unit,
            Name = specdef.Name,
            ProductGroupId = specdef.ProductGroupId.ToDocumentId(DocumentType.ProductGroup)
        };
        if (spec.BoolValue != null)
        {
            specification.BoolValue = spec.BoolValue;
        }
        if (spec.NumberValue != null)
        {
            specification.NumberValue = spec.NumberValue;
        }
        if (spec.StringValue != null)
        {
            if (spec.StringValue.StartsWith("[") && spec.StringValue.EndsWith("]"))
                specification.ArrayValues = JsonConvert.DeserializeObject<string[]>(spec.StringValue);
            else
                specification.StringValue = spec.StringValue;
        }
        return specification;
    }
    public static ReviewDocument ToDocument(this WebReview review)
    {
        return new ReviewDocument
        {
            Id = review.Id,
            ProductId = review.ProductId.ToDocumentId(DocumentType.Product),
            PartitionKey = review.ProductId.ToDocumentId(DocumentType.Product),
            Score = review.Score,
            Text = review.Text,
            ReviewUrl = review.ReviewUrl,
            ReviewType = (int)review.ReviewType
        };
    }
}
