// File: src/ShoukatSons.Services/Interfaces/Dtos/PosDocumentDto.cs
using System;
using System.Collections.Generic;

namespace ShoukatSons.Services.Interfaces.Dtos
{
    public class PosDocumentDto
    {
        public Guid Id { get; set; }
        public string Number { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public DateTime CreatedAtUtc { get; set; }
        public Guid? ExchangeGroupId { get; set; }

        public List<PosDocumentLineDto> Lines { get; set; } = new();
        public List<PosPaymentDto> Payments { get; set; } = new();
    }
}