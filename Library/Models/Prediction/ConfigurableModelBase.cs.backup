// D:\PROJETOS\GraphFacil\Library\Models\Prediction\ConfigurableModelBase.cs - Implementação base para modelos configuráveis
using LotoLibrary.Interfaces;
using System;
using System.Collections.Generic;

namespace LotoLibrary.Models.Prediction
{
    /// <summary>
    /// Classe base que implementa IConfigurableModel
    /// Facilita a criação de modelos configuráveis
    /// </summary>
    public abstract class ConfigurableModelBase : IConfigurableModel
    {
        #region Protected Fields
        protected Dictionary<string, object> _currentParameters;
        protected Dictionary<string, object> _defaultParameters;
        protected Dictionary<string, string> _parameterDescriptions;
        protected Dictionary<string, List<object>> _allowedValues;
        #endregion

        #region Constructor
        protected ConfigurableModelBase()
        {
            _currentParameters = new Dictionary<string, object>();
            _defaultParameters = new Dictionary<string, object>();
            _parameterDescriptions = new Dictionary<string, string>();
            _allowedValues = new Dictionary<string, List<object>>();

            InitializeParameters();
            ResetToDefaults();
        }
        #endregion

        #region IConfigurableModel Implementation
        public virtual Dictionary<string, object> CurrentParameters =>
            new Dictionary<string, object>(_currentParameters);

        public virtual Dictionary<string, object> DefaultParameters =>
            new Dictionary<string, object>(_defaultParameters);

        public virtual void UpdateParameters(Dictionary<string, object> parameters)
        {
            if (parameters == null) return;

            foreach (var param in parameters)
            {
                if (_defaultParameters.ContainsKey(param.Key))
                {
                    _currentParameters[param.Key] = param.Value;
                }
                else
                {
                    throw new ArgumentException($"Parâmetro '{param.Key}' não é válido para este modelo");
                }
            }

            OnParametersUpdated();
        }

        public virtual bool ValidateParameters(Dictionary<string, object> parameters)
        {
            if (parameters == null) return false;

            foreach (var param in parameters)
            {
                // Verificar se o parâmetro existe
                if (!_defaultParameters.ContainsKey(param.Key))
                {
                    return false;
                }

                // Verificar valores permitidos se definidos
                if (_allowedValues.ContainsKey(param.Key))
                {
                    var allowedValues = _allowedValues[param.Key];
                    if (allowedValues != null && !allowedValues.Contains(param.Value))
                    {
                        return false;
                    }
                }

                // Validação específica do modelo
                if (!ValidateSpecificParameter(param.Key, param.Value))
                {
                    return false;
                }
            }

            return true;
        }

        public virtual string GetParameterDescription(string parameterName)
        {
            return _parameterDescriptions.TryGetValue(parameterName, out var description)
                ? description
                : $"Parâmetro {parameterName}";
        }

        public virtual List<object> GetAllowedValues(string parameterName)
        {
            return _allowedValues.TryGetValue(parameterName, out var values)
                ? new List<object>(values)
                : null;
        }

        public virtual void ResetToDefaults()
        {
            _currentParameters.Clear();
            foreach (var param in _defaultParameters)
            {
                _currentParameters[param.Key] = param.Value;
            }

            OnParametersUpdated();
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Inicializa os parâmetros padrão do modelo - deve ser implementado pelas classes filhas
        /// </summary>
        protected abstract void InitializeParameters();

        /// <summary>
        /// Validação específica de parâmetros - pode ser sobrescrito pelas classes filhas
        /// </summary>
        protected virtual bool ValidateSpecificParameter(string parameterName, object value)
        {
            return true;
        }

        /// <summary>
        /// Chamado quando parâmetros são atualizados - pode ser sobrescrito pelas classes filhas
        /// </summary>
        protected virtual void OnParametersUpdated()
        {
            // Implementação vazia - classes filhas podem sobrescrever
        }

        /// <summary>
        /// Helper para adicionar parâmetro padrão
        /// </summary>
        protected void AddParameter(string name, object defaultValue, string description = "", List<object> allowedValues = null)
        {
            _defaultParameters[name] = defaultValue;
            _parameterDescriptions[name] = description;

            if (allowedValues != null)
            {
                _allowedValues[name] = allowedValues;
            }
        }

        /// <summary>
        /// Helper para obter valor de parâmetro tipado
        /// </summary>
        protected T GetParameter<T>(string name, T defaultValue = default(T))
        {
            if (_currentParameters.TryGetValue(name, out var value) && value is T)
            {
                return (T)value;
            }
            return defaultValue;
        }

        /// <summary>
        /// Helper para definir valor de parâmetro
        /// </summary>
        protected void SetParameter(string name, object value)
        {
            if (_defaultParameters.ContainsKey(name))
            {
                _currentParameters[name] = value;
                OnParametersUpdated();
            }
        }

        void IConfigurableModel.SetParameter(string parameterName, object value)
        {
            SetParameter(parameterName, value);
        }
        #endregion
    }
}
