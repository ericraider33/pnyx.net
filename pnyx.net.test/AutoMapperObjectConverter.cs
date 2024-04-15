using System.Collections.Generic;
using AutoMapper;
using pnyx.net.api;

namespace pnyx.net.test;

public class AutoMapperObjectConverter<TEntity> : IObjectConverterFromNameValuePair
{
    public IMapper mapper { get; set; }
    
    public object nameValuePairToObject(IDictionary<string, object> row)
    {
        TEntity entity = mapper.Map<TEntity>(row);
        return entity;
    }

    public IDictionary<string, object> objectToNameValuePair(object obj)
    {
        IDictionary<string, object> nameValuePair = mapper.Map<IDictionary<string, object>>(obj);
        return nameValuePair;
    }
}