namespace GigaIRC.Client.WPF.Completion
{
    public interface ITabCompletion
    {
        bool TabPressed();
        void TextChanged();
        void ListChanged();
    }
}