using UnityEngine;
using System;
using TMPro; 

public class Elemental : MonoBehaviour
{
    public TMP_InputField inputField; 
    public TMP_Text resultText;        
    private bool[] patterns = new bool[8]; 

  
    public void ConvertToBinary()
    {
        int number;
        if (int.TryParse(inputField.text, out number) && number >= 0 && number <= 255)
        {
            string binary = Convert.ToString(number, 2).PadLeft(8, '0');

            for (int i = 0; i < 8; i++)//for para asignar al array que se asignará a los patrones
            {
                patterns[i] = binary[i] == '1'; 
            }

            resultText.text = $"Número: {number}\nBinario: {binary}\nPatrones: {string.Join(", ", patterns)}";
        }
        else
        {
            resultText.text = "Por favor, ingresa un número entre 0 y 255.";
        }
    }
}
