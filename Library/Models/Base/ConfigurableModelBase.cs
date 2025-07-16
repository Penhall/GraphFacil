using System;
using System.Collections.Generic;
using LotoLibrary.Interfaces;

namespace LotoLibrary.Models.Base;

    public abstract class ConfigurableModelBase : IConfigurableModel
    {
        protected Dictionary<string, object> _currentParameters = new();
        protected Dictionary<string, object> _defaultParameters = new();

        public virtual void SetParameter(string parameterName, object value)
        {
            if (_currentParameters.ContainsKey(parameterName))
            {
                _currentParameters[parameterName] = value;
                OnParametersUpdated();
            }
        }

    public virtual Dictionary<string, object> CurrentParameters => _currentParameters;
    public virtual Dictionary<string, object> DefaultParameters => _defaultParameters;

    // Mudança: void em vez de bool para corresponder à interface
    public virtual void UpdateParameters(Dictionary<string, object> parameters)
    {
        if (ValidateParameters(parameters))
        {
            _currentParameters = new Dictionary<string, object>(parameters);
        }
    }

    public virtual bool ValidateParameters(Dictionary<string, object> parameters)
    {
        return parameters != null;
    }

    public virtual string GetParameterDescription(string parameterName)
    {
        return $"Descrição do parâmetro {parameterName}";
    }

    public virtual List<object> GetAllowedValues(string parameterName)
    {
        return new List<object>();
    }

    public virtual void ResetToDefaults()
    {
        _currentParameters = new Dictionary<string, object>(_defaultParameters);
    }

    protected virtual void InitializeDefaultParameters()
    {
        _defaultParameters["WindowSize"] = 50;
        _defaultParameters["Threshold"] = 0.5;
    }

    protected virtual void OnParametersUpdated()
    {
        // Implementação padrão vazia
    }
}
