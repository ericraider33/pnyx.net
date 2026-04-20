using System;
using System.Collections.Generic;
using AutoMapper;
using pnyx.net.api;

namespace pnyx.net.test.processors;

public class AutoMapperObjectConverter<TEntity> : IObjectConverterFromNameValuePair
{
    public IMapper mapper { get; }

    public AutoMapperObjectConverter(IMapper mapper)
    {
        this.mapper = mapper;
    }

    public object nameValuePairToObject(IDictionary<string, object?> row)
    {
        TEntity? entity = mapper.Map<TEntity>(row);
        if (entity == null)
            throw new NullReferenceException($"Mapped entity cannot be null for type: {typeof(TEntity).Name}");
        
        return entity;
    }

    public IDictionary<string, object?> objectToNameValuePair(object obj)
    {
        IDictionary<string, object?> nameValuePair = mapper.Map<IDictionary<string, object?>>(obj);
        return nameValuePair;
    }
}