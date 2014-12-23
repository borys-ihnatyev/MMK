using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using MMK.Marking;
using MMK.SwiftSearch.SearchHandlers;
using MMK.Wpf.Providers;
using MMK.Wpf.Providers.Key;

namespace MMK.SwiftSearch
{
    public partial class SearchWindow
    {
        private readonly double goldenRatio = 1 - ((1 + Math.Sqrt(5))/2 - 1);

        private readonly GlobalKeyShortcutProvider keyShortcutProvider;
        private readonly GlobalShortcutProviderCollection shortcutProviderCollection;

        private bool isChangingSearchBoxManually;

        private int lastInsertIndex = -1;
        private int lastInsertLength = -1;

        public SearchWindow()
        {
            InitializeComponent();
            Loaded += OnLoaded;

            shortcutProviderCollection = new GlobalShortcutProviderCollection(this);
            keyShortcutProvider = new GlobalKeyShortcutProvider();
        }

        public SearchWindow(string search) : this()
        {
            Search = search;
        }

        public string Search
        {
            get { return SearchTextBox.Text; }
            set
            {
                value = SanitizeSearchText(value);
                if (value == SearchTextBox.Text) return;

                SearchTextBox.Text = value;
                SearchTextBox.CaretIndex = value.Length;
                RecalcWindowWidth();
            }
        }

        private static string SanitizeSearchText(string value)
        {
            return value.Replace("\r", "")
                .Replace("\n", " ")
                .Trim();
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            SetAmazingPosition();

            LoadShortcutCollection();
            (keyShortcutProvider as IGlobalShortcutProvider).SetWindow(this);

            keyShortcutProvider.StartListen();
            shortcutProviderCollection.StartListen();

            LoadSearchTextBoxText();

            SearchTextBox.Focus();
        }

        private void SetAmazingPosition()
        {
            double screenHeight = SystemParameters.PrimaryScreenHeight;

            Top = screenHeight*goldenRatio - Height;
        }

        private void LoadSearchTextBoxText()
        {
            if (!string.IsNullOrWhiteSpace(Search)) return;

            string[] clArgs = Environment.GetCommandLineArgs();
            if (clArgs.Length <= 1) return;

            string search = string.Join(" ", clArgs, 1, clArgs.Length - 1);

            Search = search;
        }

        private void LoadShortcutCollection()
        {
            keyShortcutProvider.HotKeyPressed += InsertSearchTextBoxTextParalelKeys;

            shortcutProviderCollection.Add(
                KeyModifyers.None,
                (int) Keys.Enter,
                SearchCommandAction
                );

            shortcutProviderCollection.Add(
                KeyModifyers.None,
                (int) Keys.Escape,
                CancelCommandAction
                );

            shortcutProviderCollection.Add(
                KeyModifyers.Ctrl,
                (int) Keys.Space,
                CancelCommandAction
                );


            shortcutProviderCollection.Add(
                KeyModifyers.Alt,
                (int) Keys.G,
                () => InsertIntoSearchBoxText(GoodleSearchHandler.Identifyer)
                );

            shortcutProviderCollection.Add(
                KeyModifyers.Alt,
                (int) Keys.V,
                () => InsertIntoSearchBoxText(VkSearchHandler.Identifyer)
                );

            shortcutProviderCollection.Add(
                KeyModifyers.Alt,
                (int) Keys.Z,
                () => InsertIntoSearchBoxText(ZaycevNetSearchHandler.Identifyer)
                );

            shortcutProviderCollection.Add(
                KeyModifyers.Alt,
                (int) Keys.A,
                () => InsertIntoSearchBoxText(GetAllIdentifyers()));

            shortcutProviderCollection.Add(
                KeyModifyers.Ctrl | KeyModifyers.Shift,
                (int) Keys.H,
                () => InsertIntoSearchBoxTextAsHashTag("house")
                );

            shortcutProviderCollection.Add(
                KeyModifyers.Ctrl | KeyModifyers.Shift,
                (int)Keys.D,
                () => InsertIntoSearchBoxTextAsHashTag("deep")
                );

            shortcutProviderCollection.Add(
                KeyModifyers.Ctrl | KeyModifyers.Shift,
                (int)Keys.N,
                () => InsertIntoSearchBoxTextAsHashTag("newdisco")
                );

            shortcutProviderCollection.Add(
                KeyModifyers.Ctrl | KeyModifyers.Shift,
                (int) Keys.R,
                () => InsertIntoSearchBoxText("<#dance|#pop|#spec>")
                );

            shortcutProviderCollection.Add(
                KeyModifyers.Ctrl | KeyModifyers.Shift,
                (int) Keys.P,
                () => InsertIntoSearchBoxTextAsHashTag("pop")
                );

            shortcutProviderCollection.Add(
                KeyModifyers.Ctrl | KeyModifyers.Shift,
                (int) Keys.S,
                () => InsertIntoSearchBoxTextAsHashTag("spec")
                );

            shortcutProviderCollection.Add(
                KeyModifyers.Ctrl | KeyModifyers.Shift,
                (int)Keys.U,
                () => InsertIntoSearchBoxTextAsHashTag("urban")
                );

            shortcutProviderCollection.Add(
                KeyModifyers.Ctrl | KeyModifyers.Shift,
                (int)Keys.E,
                () => InsertIntoSearchBoxTextAsHashTag("electronic")
                );

            shortcutProviderCollection.Add(
                KeyModifyers.Ctrl | KeyModifyers.Shift,
                (int) Keys.L,
                () => InsertIntoSearchBoxTextAsHashTag("slow")
                );
        }

        private static string GetAllIdentifyers()
        {
            return string.Join(
                " ",
                GoodleSearchHandler.Identifyer,
                VkSearchHandler.Identifyer,
                ZaycevNetSearchHandler.Identifyer
                );
        }


        private void InsertSearchTextBoxTextParalelKeys(Key key)
        {
            string keyStr = string.Format("<#{0}|#{1}>", key, CircleOfFifths.GetParalel(key));
            InsertIntoSearchBoxText(keyStr);
        }

        private void InsertIntoSearchBoxTextAsHashTag(string value)
        {
            string insertValue = new HashTag(value).ToString();

            InsertIntoSearchBoxText(insertValue);
        }

        private void InsertIntoSearchBoxText(string insertValue)
        {
            isChangingSearchBoxManually = true;

            int insertIndex = SearchTextBox.CaretIndex;

            insertValue += " ";

            if (insertIndex == 0)
            {
                SearchTextBox.Text = insertValue.TrimStart() + SearchTextBox.Text;
            }
            else
            {
                if (SearchTextBox.Text[insertIndex - 1] != ' ')
                    insertValue = " " + insertValue;

                if (insertIndex >= SearchTextBox.Text.Length)
                    SearchTextBox.Text += insertValue;
                else
                    SearchTextBox.Text = SearchTextBox.Text.Insert(insertIndex, insertValue);
            }

            SearchTextBox.CaretIndex = insertIndex + insertValue.Length;


            isChangingSearchBoxManually = false;

            SetLastInsertParams(insertIndex, insertValue);
        }


        private void SearchCommandAction()
        {
            if (!string.IsNullOrWhiteSpace(Search.Trim()))
                new MultiSearchHandler(Search).Search();
            Close();
        }

        private void CancelCommandAction()
        {
            Close();
        }


        private void SearchTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            if (CanRemoveLastHotkeyInsert(e.Changes.Last()))
                RemoveLastHotkeyInsert();

            ResetLastHotkeyInsert();
            RecalcWindowWidth();
        }

        private bool CanRemoveLastHotkeyInsert(TextChange change)
        {
            return IsLastInsertedByHotkey()
                   && !isChangingSearchBoxManually
                   && change.Offset == (lastInsertIndex + lastInsertLength - 1)
                   && change.RemovedLength == 1;
        }


        private bool IsLastInsertedByHotkey()
        {
            return lastInsertIndex > -1 && lastInsertLength > 0;
        }

        private void SetLastInsertParams(int caretIndex, string insertValue)
        {
            lastInsertIndex = caretIndex;
            lastInsertLength = insertValue.Length;
        }

        private void ResetLastHotkeyInsert()
        {
            lastInsertIndex = -1;
            lastInsertLength = -1;
        }

        private void RemoveLastHotkeyInsert()
        {
            isChangingSearchBoxManually = true;

            Search = SearchTextBox.Text.Remove(lastInsertIndex, lastInsertLength - 1);

            isChangingSearchBoxManually = false;
        }

        private void RecalcWindowWidth()
        {
            double textWidth = SearchTextBox.MeasureText().Width;

            if (textWidth < MinWidth)
                textWidth = MinWidth;
            else if (textWidth > MaxWidth)
                textWidth = MaxWidth;

            Width = textWidth + SearchTextBox.Margin.Left + SearchTextBox.Margin.Right + 10;

            Left = (1366 - Width)/2;
        }
    }
}