// File: src/ShoukatSons.Services/PosConversionService.cs
using System.Collections.Generic;
using AutoMapper;
using ShoukatSons.Core.POS;
using ShoukatSons.Data.Entities;

namespace ShoukatSons.Services
{
    public class PosConversionService
    {
        private readonly IMapper _mapper;

        public PosConversionService(IMapper mapper)
        {
            _mapper = mapper;
        }

        public PosDocument ConvertToDocument(Document entity)
            => _mapper.Map<PosDocument>(entity);

        public Document ConvertToEntity(PosDocument doc)
            => _mapper.Map<Document>(doc);

        public List<PosDocument> ConvertToDocuments(IEnumerable<Document> entities)
            => _mapper.Map<List<PosDocument>>(entities);

        public List<Document> ConvertToEntities(IEnumerable<PosDocument> docs)
            => _mapper.Map<List<Document>>(docs);
    }
}