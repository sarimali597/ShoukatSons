// File: src/ShoukatSons.Application/Mappings/PosMappings.cs
using System;
using System.Linq;
using ShoukatSons.Core.POS;
using ShoukatSons.Services.Interfaces.Dtos;
namespace ShoukatSons.Application.Mappings
{
    public static class PosMappings
    {
        public static PosDocumentDto ToDto(this PosDocument doc)
        {
            if (doc == null) return null;

            return new PosDocumentDto
            {
                Id = doc.Id,
                Number = doc.Number,
                CustomerName = doc.CustomerName,
                CreatedAtUtc = doc.CreatedAtUtc,
                ExchangeGroupId = doc.ExchangeGroupId,

                // Lines
                Lines = doc.Lines?.Select(l => new PosDocumentLineDto
                {
                    Id = l.Id,
                    ProductId = l.ProductId,
                    ProductName = l.ProductName,
                    Quantity = l.Quantity,
                    UnitPrice = l.UnitPrice,
                    LineDiscount = l.LineDiscount,
                    Product = l.Product, // still 'object' unless replaced with real type
                    LineTotal = l.LineTotal
                }).ToList(),

                // Payments
                Payments = doc.Payments?.Select(p => new PosPaymentDto
                {
                    Id = p.Id,
                    PosDocumentId = p.PosDocumentId,
                    Method = p.Method,
                    Amount = p.Amount,
                    Currency = p.Currency,
                    PaidAtUtc = p.PaidAtUtc,
                    CreatedByUserId = p.CreatedByUserId
                }).ToList()
            };
        }

        public static PosDocument ToEntity(this PosDocumentDto dto)
        {
            if (dto == null) return null;

            return new PosDocument
            {
                Id = dto.Id,
                Number = dto.Number,
                CustomerName = dto.CustomerName,
                CreatedAtUtc = dto.CreatedAtUtc,
                ExchangeGroupId = dto.ExchangeGroupId,

                // Lines
                Lines = dto.Lines?.Select(l => new PosDocumentLine
                {
                    Id = l.Id,
                    ProductId = l.ProductId,
                    ProductName = l.ProductName,
                    Quantity = l.Quantity,
                    UnitPrice = l.UnitPrice,
                    LineDiscount = l.LineDiscount,
                    Product = l.Product
                    // No LineTotal assignment — computed automatically
                }).ToList(),

                // Payments
                Payments = dto.Payments?.Select(p => new PosPayment
                {
                    Id = p.Id,
                    PosDocumentId = p.PosDocumentId,
                    Method = p.Method,
                    Amount = p.Amount,
                    Currency = p.Currency,
                    PaidAtUtc = p.PaidAtUtc,
                    CreatedByUserId = p.CreatedByUserId
                }).ToList()
            };
        }
    }
}