using TMPro;

public class TMP_InputValidation : TMP_InputValidator
{
    public override char Validate(ref string text, ref int pos, char ch)
    {
        // Check if user input is a rich text tag start character ("<")
        if (!UIChat.singleton.inputFromBackend && (ch == '<' || ch == '>'))
        {
            // Prevent user input of rich text tags
            return '\0'; // Return null character to suppress input
        }
        text += ch;
        pos += 1;
        return ch;
    }
}
