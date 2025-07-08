
// D:\PROJETOS\GraphFacil\Dashboard\Extensions\ViewModelExtensions.cs
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Dashboard.Extensions
{
    /// <summary>
    /// Extensões úteis para ViewModels
    /// </summary>
    public static class ViewModelExtensions
    {
        /// <summary>
        /// Helper para PropertyChanged com validação
        /// </summary>
        public static bool SetProperty<T>(this INotifyPropertyChanged source, ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(field, value)) return false;

            field = value;

            if (source is INotifyPropertyChanged notifySource)
            {
                var handler = typeof(INotifyPropertyChanged).GetField("PropertyChanged",
                    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)?.GetValue(notifySource) as PropertyChangedEventHandler;

                handler?.Invoke(source, new PropertyChangedEventArgs(propertyName));
            }

            return true;
        }
    }
}