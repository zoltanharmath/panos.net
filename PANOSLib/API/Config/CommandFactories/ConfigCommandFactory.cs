﻿namespace PANOS
{
    public class ConfigCommandFactory : IConfigCommandFactory
    {
        private readonly ApiUriFactory apiUriFactory;
        private readonly IConfigPostKeyValuePairFactory apiPostKeyValuePairFactory;
        
        public ConfigCommandFactory(
            ApiUriFactory apiUriFactory,
            IConfigPostKeyValuePairFactory apiPostKeyValuePairFactory)
        {
            this.apiUriFactory = apiUriFactory;
            this.apiPostKeyValuePairFactory = apiPostKeyValuePairFactory;
        }

        public ICommand<TApiResponse> CreateGetAll<TApiResponse>(
            string schemaName,
            ConfigTypes configType = ConfigTypes.Running) where TApiResponse : ApiResponse => 
                new Command<TApiResponse>(apiUriFactory.Create(), apiPostKeyValuePairFactory.CreateGetAll(schemaName, configType));
        

        public ICommand<TApiResponse> CreateGetSingle<TApiResponse>(
            string schemaName,
            string name, ConfigTypes configType = ConfigTypes.Running) where TApiResponse : ApiResponse => 
                new Command<TApiResponse>(apiUriFactory.Create(), apiPostKeyValuePairFactory.CreateGetSingle(schemaName, name, configType));
        

        public ICommand<ApiResponseWithMessage> CreateSet(FirewallObject firewallObject) => 
            new Command<ApiResponseWithMessage>(
                apiUriFactory.Create(),
                apiPostKeyValuePairFactory.CreateSet(firewallObject));
        
        public ICommand<ApiResponseWithMessage> CreateDelete(string schemaName, string name) => 
            new Command<ApiResponseWithMessage>(
                apiUriFactory.Create(),
                apiPostKeyValuePairFactory.CreateDelete(schemaName, name));
        

        public ICommand<ApiResponseWithMessage> CreateRename(string schemaName, string oldName, string newName) => 
            new Command<ApiResponseWithMessage>(
                apiUriFactory.Create(),
                apiPostKeyValuePairFactory.CreateRename(schemaName, oldName, newName));
    }
}
