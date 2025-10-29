using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Calculator : MonoBehaviour
{

    public TMP_Text expressionText;
    public TMP_Text resultText;

    private string currentExpression = "";
    private bool showingResult = false;

    public void Start()
    {
        UpdateDisplay();
    }

    // ---------------- Button Functions ----------------

    public void OnNumberClick(string number)
    {
        if (showingResult)
        {
            currentExpression = "";
            showingResult = false;
        }

        currentExpression += number;
        UpdateDisplay();
    }

    public void OnOperatorClick(string op)
    {
        if (string.IsNullOrEmpty(currentExpression))
            return;

        char last = currentExpression[currentExpression.Length - 1];

        // Replace last operator if user clicks two operators in a row

        if (IsOperator(last))
        {
            currentExpression = currentExpression.Substring(0, currentExpression.Length - 1);
        }

        currentExpression += op;
        showingResult = false;
        UpdateDisplay();
    }

    public void OnDecimalClick()
    {
        if (showingResult)
        {
            currentExpression = "0";
            showingResult = false;
        }

        string[] parts = SplitByOperators(currentExpression);
        string lastNum = parts.Length > 0 ? parts[parts.Length - 1] : "";

        if (!lastNum.Contains("."))
        {
            if (string.IsNullOrEmpty(currentExpression) || IsOperator(currentExpression[^1]))
                currentExpression += "0.";

            else
                currentExpression += ".";
        }

        UpdateDisplay();
    }

    public void OnEqualsClick()
    {
        if (string.IsNullOrEmpty(currentExpression))
            return;

        try
        {
            double answer = EvaluateExpression(currentExpression);
            resultText.text = answer.ToString();
            showingResult = true;
        }
        catch (Exception e)
        {
            resultText.text = "Error";
            Debug.LogError("Error calculating: " + e.Message);
        }
    }

    public void OnClearClick()
    {
        if (currentExpression.Length > 0)
        {
            currentExpression = currentExpression.Substring(0, currentExpression.Length - 1);
            UpdateDisplay();
        }
    }

    public void OnResetClick()
    {
        currentExpression = "";
        resultText.text = "0";
        showingResult = false;
        UpdateDisplay();
    }

    // ---------------- Expression Logic ----------------

    private double EvaluateExpression(string expr)
    {
        List<double> numbers = new List<double>();
        List<char> ops = new List<char>();

        ParseExpression(expr, numbers, ops);

        // Handle * and / first

        for (int i = 0; i < ops.Count; i++)
        {
            if (ops[i] == '*' || ops[i] == '/')
            {
                double res = ApplyOperation(numbers[i], numbers[i + 1], ops[i]);
                numbers[i] = res;
                numbers.RemoveAt(i + 1);
                ops.RemoveAt(i);
                i--;
            }
        }

        // Then + and -

        for (int i = 0; i < ops.Count; i++)
        {
            if (ops[i] == '+' || ops[i] == '-')
            {
                double res = ApplyOperation(numbers[i], numbers[i + 1], ops[i]);
                numbers[i] = res;
                numbers.RemoveAt(i + 1);
                ops.RemoveAt(i);
                i--;
            }
        }

        return numbers[0];
    }

    private void ParseExpression(string expr, List<double> numbers, List<char> ops)
    {
        string currentNum = "";

        foreach (char c in expr)
        {
            if (char.IsDigit(c) || c == '.')
            {
                currentNum += c;
            }
            else if (IsOperator(c))
            {
                if (!string.IsNullOrEmpty(currentNum))
                {
                    numbers.Add(double.Parse(currentNum));
                    currentNum = "";
                }
                ops.Add(c);
            }
        }

        if (!string.IsNullOrEmpty(currentNum))
            numbers.Add(double.Parse(currentNum));
    }

    private double ApplyOperation(double a, double b, char op)
    {
        return op switch
        {
            '+' => a + b,
            '-' => a - b,
            '*' => a * b,
            '/' => b == 0 ? throw new DivideByZeroException("Can't divide by zero") : a / b,
            _ => throw new ArgumentException("Invalid operator: " + op)
        };
    }

    // ---------------- Helpers ----------------

    private bool IsOperator(char c)
    {
        return c == '+' || c == '-' || c == '*' || c == '/';
    }

    private string[] SplitByOperators(string expr)
    {
        List<string> parts = new List<string>();
        string current = "";

        foreach (char c in expr)
        {
            if (IsOperator(c))
            {
                if (!string.IsNullOrEmpty(current))
                {
                    parts.Add(current);
                    current = "";
                }
            }
            else
            {
                current += c;
            }
        }

        if (!string.IsNullOrEmpty(current))
            parts.Add(current);

        return parts.ToArray();
    }

    private void UpdateDisplay()
    {
        expressionText.text = string.IsNullOrEmpty(currentExpression) ? "0" : currentExpression;

        if (!showingResult)
            resultText.text = "0";
    }
}
