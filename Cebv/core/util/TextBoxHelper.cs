using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Cebv.core.util.snackbar;
using Microsoft.Extensions.DependencyInjection;
using Wpf.Ui.Controls;
using TextBox = System.Windows.Controls.TextBox;

namespace Cebv.core.util;

public class TextBoxHelper
{
  

    private static ISnackbarService _snackbarService = App.Current.Services.GetService<ISnackbarService>()!;
    /// <summary>
    /// Método auxiliar para verificar si el TextBox está dentro de un DatePicker.
    /// </summary>
    /// <param name="depObj"></param>
    /// <returns></returns>   
    private static bool IsDatePicker(DependencyObject depObj)
    {
        while (depObj != null)
        {
            if (depObj is DatePicker)
            {
                return true;
            }
            depObj = VisualTreeHelper.GetParent(depObj);
        }
        return false;
    }

    /// <summary>
    /// Maneja el evento SelectedDateChanged para restringir la selección de fechas futuras.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public static void DatePickerSelectedDateChanged(object sender, SelectionChangedEventArgs e)
    {
        DatePicker datePicker = sender as DatePicker;
        // Verificar si el DatePicker tiene el Tag "Exclude"
        if (datePicker.Tag?.ToString() == "Exclude")
        {
            return;
        }

        if (datePicker != null)
        {
            datePicker.DisplayDateEnd = DateTime.Now;
            datePicker.SelectedDateChanged -= DatePickerSelectedDateChanged;

            if (datePicker.SelectedDate.HasValue && datePicker.SelectedDate.Value > DateTime.Now)
            {
                datePicker.SelectedDate = DateTime.Now;
            }
        }
    }

    /// <summary>
    /// Eventos que se dispara cuando el texto de un TextBox cambia
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public static void UpperCaseText(object sender, TextChangedEventArgs e)
    { 
        TextBox textBox = (sender as TextBox)!;
        
        // Verificar si el TextBox tiene el Tag "Exclude" o si está dentro de un DatePicker
        if (IsDatePicker(textBox) || textBox.Tag?.ToString() == "Exclude" || textBox.Tag?.ToString() == "Mail")
        {
            return;
        }
        
        // Convertir el texto a mayúsculas
        if (textBox != null)
        {
            int caretIndex = textBox.CaretIndex;
            textBox.Text = textBox.Text.ToUpper();
            textBox.CaretIndex = caretIndex;
        }
        
        // Guardar la posición actual del cursor
        int cursorPosition = textBox.SelectionStart;

        // Restaurar la posición del cursor
        textBox.SelectionStart = cursorPosition;
        textBox.SelectionLength = 0;
    }

    /// <summary>
    /// Evento que se dispara cuando se presiona una tecla en un TextBox
    /// y permite solo ciertos caracteres según el Tag del TextBox.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public static void PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        TextBox textBox = (sender as TextBox)!;
        string pattern;

        // Verificar si el TextBox tiene el Tag "Exclude" o si está dentro de un DatePicker
        if (IsDatePicker(textBox) || textBox.Tag?.ToString() == "Exclude")
        {
            return;
        }

        switch (textBox?.Tag?.ToString())
        {
            case "Number":
                // Patrón para permitir solo números
                pattern = @"[^0-9]";
          
                // No permitir números negativos
                if (textBox.Text.Contains("-") || e.Text == "-")
                {
                    e.Handled = true;
                    return;
                }
                break;
            case "Letter":
                // Patrón para permitir solo letras
                pattern = @"[^A-ZÑ.]";
                break;
            case "Units":
                // Patrón para permitir solo numeros y caracteres de unidad de medida
                pattern = @"[^0-9.,]";
                // Permitir solo un punto decimal o coma
                if (e.Text == "." || e.Text == ",")
                {
                    if (textBox.Text.Contains(".") || textBox.Text.Contains(","))
                    {
                        e.Handled = true;
                    }
                    return;
                }
                // No permitir números negativos
                if (textBox.Text.Contains("-") || e.Text == "-")
                {
                    e.Handled = true;
                    return;
                }
                break;
            case "Time":
                //Solo números y caracteres de tiempo
                pattern = @"[^\d{2}:\d{2}$]";
                textBox.MaxLength = 5;
                break;
            case "Date":
                //Solo números y caracteres de fecha
                pattern = @"[^\d{2}/\d{2}/\d{4}$]";
                textBox.MaxLength = 10;
                break;
            case "Mail":
                // Patrón para permitir letras, números y caracteres especiales de correo electrónico
                pattern = @"[^a-zA-ZñÑ0-9@._-]";
                break;
            default:
                // Patrón para permitir letras y la Ñ
                pattern = @"[^A-ZÑ0-9,]";
                break;
        }

        if (Regex.IsMatch(e.Text.ToUpper(), pattern))
        {
            e.Handled = true;
        }
    }
    
    /// <summary>
    /// Evento que se dispara cuando el texto de un TextBox cambia
    /// y permite completar automáticamente el texto según el Tag del TextBox.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public static void AutoCompleted(object sender, TextChangedEventArgs e)
    {
      

        TextBox textBox = (sender as TextBox)!;
        
        // Verificar si el TextBox tiene el Tag "Exclude" o si está dentro de un DatePicker
        if (IsDatePicker(textBox) || textBox.Tag?.ToString() == "Exclude")
        {
            return;
        }
        
        if (textBox?.Tag?.ToString() == "Date")
        {
            if ((textBox.Text.Length == 2 || textBox.Text.Length == 5) && !textBox.Text.EndsWith("/"))
            {
                textBox.Text += "/";
                textBox.CaretIndex = textBox.Text.Length;
            }
        }else if (textBox?.Tag?.ToString() == "Time")
        {
            if (textBox.Text.Length == 2 && !textBox.Text.EndsWith(":"))
            {
                textBox.Text += ":";
                textBox.CaretIndex = textBox.Text.Length;
            }
        }
    }
    
    /// <summary>
    /// Evento que se dispara cuando un TextBox pierde el foco,
    /// elimina los espacios finales e iniciales y
    /// reemplaza múltiples espacios consecutivos con un solo espacio.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public static void ValidText(object sender, RoutedEventArgs e)
    {
        int _contadorerrores=0;
        string error = String.Empty;
        List<string> errores = new List<string>();
        TextBox textBox = (sender as TextBox)!;
        if (textBox.Text != "")
        {
            // Verificar si el TextBox tiene el Tag "Exclude" o si está dentro de un DatePicker
            if (IsDatePicker(textBox) || textBox.Tag?.ToString() == "Exclude")
            {
                return;
            }

            if (textBox?.Tag?.ToString() == "Time")
            {
                if (!Regex.IsMatch(textBox.Text, @"^([0-1][0-9]|2[0-3]):([0-5][0-9])$"))
                {
                    error = "Por favor ingrese formato valido: \"HH:MM\" \nEjemplo: \"23:59\"";
                    errores.Add(error);
                    textBox.BorderBrush = new SolidColorBrush(Colors.Orange);
                    _contadorerrores++;
                }
                else
                {
                    //Resetea el borde al que esta por defecto por wpf UI
                    textBox.ClearValue(Border.BorderBrushProperty);
                    _contadorerrores--;

                }
            }

            if (textBox?.Tag?.ToString() == "Date")
            {
                if (!Regex.IsMatch(textBox.Text, @"^((0[1-9]|[12][0-9]|3[01])|99)/((0[1-9]|1[0-2])|99)/\d{4}$"))
                {

                    error = "Por favor ingrese formato valido: \"DD/MM/AAAA\" \nEjemplo: \"31/12/2021\"";
                    errores.Add(error);
                    textBox.BorderBrush = new SolidColorBrush(Colors.Orange);
                    _contadorerrores++;

                }
                else
                {
                    textBox.ClearValue(Border.BorderBrushProperty);
                    _contadorerrores--;

                }
            }

            if (textBox?.Tag?.ToString() == "Mail")
            {
                if (!Regex.IsMatch(textBox.Text, @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$"))
                {
                    error = "Por favor ingrese un correo electrónico valido.";
                    errores.Add(error);
                    textBox.BorderBrush = new SolidColorBrush(Colors.Orange);
                    _contadorerrores++;

                }
                else
                {
                    textBox.ClearValue(Border.BorderBrushProperty);
                    _contadorerrores--;

                }
            }

            if (textBox?.Tag?.ToString() == "Phone")
            {
                if (textBox.Text.Length < 8 || textBox.Text.Length > 10 || !Regex.IsMatch(textBox.Text, @"^[0-9]+$"))
                {
                    error = "El numero de telefono tiene errores.";
                    errores.Add(error);
                    textBox.BorderBrush = new SolidColorBrush(Colors.Orange);
                    _contadorerrores++;

                }
                else
                {
                    textBox.ClearValue(Border.BorderBrushProperty);
                    _contadorerrores--;//hacer caso omiso de estas aberraciones luego las quito

                }
            }

            if (textBox?.Tag?.ToString() == "CURP")
            {
                if (textBox.Text.Length != 18 || !Regex.IsMatch(textBox.Text, "^[A-Za-z0-9]+$"))
                {
                    error = "El CURP no tiene el formato correcto";
                    errores.Add(error);
                    textBox.BorderBrush = new SolidColorBrush(Colors.Orange);
                    _contadorerrores++;

                }
                else
                {
                    textBox.ClearValue(Border.BorderBrushProperty);
                    _contadorerrores--;

                }
            }

            if (textBox?.Tag?.ToString() == "CodigoPostal")
            {
                if (textBox.Text.Length != 5 || !Regex.IsMatch(textBox.Text, "^[0-9]+$"))
                {
                    error = "El Código Postal no tiene el formato correcto";
                    errores.Add(error);
                    textBox.BorderBrush = new SolidColorBrush(Colors.Orange);
                    _contadorerrores++;

                }
                else
                {
                    textBox.ClearValue(Border.BorderBrushProperty);
                    _contadorerrores--;

                }
            }
            //Cambie las tag de telefono de number a phone, se requiere reasignar tags mas especificas a cada caso



            // Eliminar espacios finales e iniciales
            string trimmedText = textBox.Text.Trim();

            // Reemplazar múltiples espacios consecutivos con un solo espacio
            string singleSpaceText = Regex.Replace(trimmedText, @"\s+", " ");

            textBox.Text = singleSpaceText;
            if (_contadorerrores > 0)
            {
                string mensaje = errores.Aggregate(string.Empty, (current, s) => current + (s + Environment.NewLine));

                _snackbarService.Show(
                    "Formato no valido",
                    mensaje,
                    ControlAppearance.Caution,
                    new SymbolIcon(SymbolRegular.Warning20),
                    new TimeSpan(0, 0, 5));

                e.Handled = true;
                textBox.BorderBrush = new SolidColorBrush(Colors.Orange);
            }
        }else{                    textBox.ClearValue(Border.BorderBrushProperty);
        }
    }
    
}