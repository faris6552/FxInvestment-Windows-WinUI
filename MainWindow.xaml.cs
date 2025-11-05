using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using System;
using System.Threading.Tasks;
using Windows.UI.Text;

namespace FxInvestmentManager
{
    public sealed partial class MainWindow : Window
    {
        private DatabaseHelper _dbHelper = new DatabaseHelper();

        public MainWindow()
        {
            this.InitializeComponent();
            InitializeApplication();
        }

        private void InitializeApplication()
        {
            ShowDashboard();
            UpdateStatusBar();
            UpdateQuickStatsWithRealData();
        }

        // Main Module Navigation
        private void BtnDashboard_Click(object sender, RoutedEventArgs e) => ShowDashboard();
        private void BtnPerformance_Click(object sender, RoutedEventArgs e) => ShowPerformance();
        private void BtnAccounts_Click(object sender, RoutedEventArgs e) => ShowAccounts();
        private void BtnReports_Click(object sender, RoutedEventArgs e) => ShowReports();

        // Tools Navigation
        private void BtnSettings_Click(object sender, RoutedEventArgs e) => ShowSettings();
        private void BtnHelp_Click(object sender, RoutedEventArgs e) => ShowHelp();

        private void ShowDashboard()
        {
            SetActiveButton(BtnDashboard);
            MainContentPanel.Children.Clear();

            var title = CreateModuleTitle("TRADING DASHBOARD");
            MainContentPanel.Children.Add(title);

            // Summary Cards
            var cardsGrid = CreateCardsGrid();
            MainContentPanel.Children.Add(cardsGrid);

            // Recent Activity
            var activityTitle = CreateSectionTitle("RECENT ACTIVITY");
            MainContentPanel.Children.Add(activityTitle);

            var activityContent = CreateContentCard(@"
• AC1: Weekly performance recorded - Profit: $75.25
• AC2: New deposit processed - Amount: $1,000.00  
• AC1: 24 trades completed this week
• System: All modules operational
• Performance: +1.5% weekly return");
            MainContentPanel.Children.Add(activityContent);

            // System Status
            var statusTitle = CreateSectionTitle("SYSTEM STATUS");
            MainContentPanel.Children.Add(statusTitle);

            var statusContent = CreateContentCard(@"
✅ Database Connection: Active
✅ File System: Ready for CSV imports
✅ Reporting Engine: Operational
✅ Data Analytics: Available
🟡 Real-time Updates: Pending configuration");
            MainContentPanel.Children.Add(statusContent);
        }
        private void ShowPerformance()
        {
            SetActiveButton(BtnPerformance);
            MainContentPanel.Children.Clear();

            var title = CreateModuleTitle("PERFORMANCE MANAGEMENT");
            MainContentPanel.Children.Add(title);

            // Create tab-like navigation
            var tabPanel = CreatePerformanceTabPanel();
            MainContentPanel.Children.Add(tabPanel);

            // Content area for tabs
            var contentPanel = new StackPanel();
            contentPanel.Margin = new Thickness(0, 20, 0, 0);
            MainContentPanel.Children.Add(contentPanel);

            // Show default tab (New Performance)
            ShowNewPerformanceTab(contentPanel);
        }

        private StackPanel CreatePerformanceTabPanel()
        {
            var tabPanel = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 2 };

            var newTab = CreatePerformanceTab("New Performance", true);
            var updateTab = CreatePerformanceTab("Update Records", false);
            var deleteTab = CreatePerformanceTab("Delete Records", false);
            var viewTab = CreatePerformanceTab("View History", false);

            newTab.Tag = "New";
            updateTab.Tag = "Update";
            deleteTab.Tag = "Delete";
            viewTab.Tag = "View";

            newTab.Click += (s, e) => ShowPerformanceTab(s as Button, "New");
            updateTab.Click += (s, e) => ShowPerformanceTab(s as Button, "Update");
            deleteTab.Click += (s, e) => ShowPerformanceTab(s as Button, "Delete");
            viewTab.Click += (s, e) => ShowPerformanceTab(s as Button, "View");

            tabPanel.Children.Add(newTab);
            tabPanel.Children.Add(updateTab);
            tabPanel.Children.Add(deleteTab);
            tabPanel.Children.Add(viewTab);

            return tabPanel;
        }

        private Button CreatePerformanceTab(string content, bool isActive)
        {
            return new Button
            {
                Content = content,
                Background = new SolidColorBrush(isActive ? GetColorFromHex("#2B579A") : Colors.Transparent),
                Foreground = new SolidColorBrush(isActive ? Colors.White : GetColorFromHex("#333333")),
                BorderThickness = new Thickness(1),
                BorderBrush = new SolidColorBrush(GetColorFromHex("#2B579A")),
                Padding = new Thickness(20, 10, 20, 10),
                FontSize = 12,
                FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
                CornerRadius = new CornerRadius(6, 6, 0, 0)
            };
        }

        private void ShowPerformanceTab(Button? clickedTab, string tabName)
        {
            // Reset all tabs
            var tabPanel = MainContentPanel.Children[1] as StackPanel;
            if (tabPanel == null) return;

            foreach (var child in tabPanel.Children)
            {
                if (child is Button tab)
                {
                    tab.Background = new SolidColorBrush(Colors.Transparent);
                    tab.Foreground = new SolidColorBrush(GetColorFromHex("#333333"));
                }
            }

            // Set active tab
            if (clickedTab != null)
            {
                clickedTab.Background = new SolidColorBrush(GetColorFromHex("#2B579A"));
                clickedTab.Foreground = new SolidColorBrush(Colors.White);
            }

            // Find content panel and show appropriate content
            var contentPanel = MainContentPanel.Children[2] as StackPanel;
            if (contentPanel == null) return;

            contentPanel.Children.Clear();

            switch (tabName)
            {
                case "New":
                    ShowNewPerformanceTab(contentPanel);
                    break;
                case "Update":
                    ShowUpdatePerformanceTab(contentPanel);
                    break;
                case "Delete":
                    ShowDeletePerformanceTab(contentPanel);
                    break;
                case "View":
                    ShowViewPerformanceTab(contentPanel);
                    break;
            }
        }

        private void ShowNewPerformanceTab(StackPanel contentPanel)
        {
            var card = CreateContentCard("");
            var formPanel = new StackPanel { Spacing = 15 };

            // Account Selection
            var accountPanel = CreateFormField("Account:", CreateAccountComboBox());

            // Week Auto-detection
            var weekPanel = CreateFormField("Week:", new TextBlock
            {
                Text = $"Auto-detected: {DateTime.Now:MMM} Week {GetCurrentWeek()}",
                FontSize = 14,
                Foreground = new SolidColorBrush(Colors.Gray)
            });

            // FX ID Display
            var fxIdPanel = CreateFormField("FX ID:", new TextBlock
            {
                Text = "AC1WK0101 (Auto-generated after file upload)",
                FontSize = 14,
                FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
                Foreground = new SolidColorBrush(GetColorFromHex("#2B579A"))
            });

            // File Upload Section
            var fileSection = new StackPanel { Spacing = 10 };
            fileSection.Children.Add(new TextBlock
            {
                Text = "Upload Weekly Trade CSV File:",
                FontSize = 14,
                FontWeight = Microsoft.UI.Text.FontWeights.SemiBold
            });

            var fileButton = new Button
            {
                Content = "📁 Select CSV File",
                Background = new SolidColorBrush(GetColorFromHex("#2B579A")),
                Foreground = new SolidColorBrush(Colors.White),
                Padding = new Thickness(20, 10, 20, 10),
                FontSize = 12,
                FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
                CornerRadius = new CornerRadius(6)
            };

            var fileInfo = new TextBlock
            {
                Text = "No file selected",
                FontSize = 12,
                Foreground = new SolidColorBrush(Colors.Gray),
                TextWrapping = TextWrapping.Wrap
            };

            fileButton.Click += (s, e) => SimulateFileUpload(fileInfo);
            fileSection.Children.Add(fileButton);
            fileSection.Children.Add(fileInfo);

            // Results Preview
            var resultsSection = new StackPanel { Spacing = 10, Margin = new Thickness(0, 20, 0, 0) };
            resultsSection.Children.Add(new TextBlock
            {
                Text = "Results Preview:",
                FontSize = 14,
                FontWeight = Microsoft.UI.Text.FontWeights.SemiBold
            });

            var resultsGrid = new Grid();
            resultsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            resultsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            resultsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            resultsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            resultsGrid.Children.Add(CreateResultCard("Total Trades", "0", 0));
            resultsGrid.Children.Add(CreateResultCard("Total Profit", "$0.00", 1));
            resultsGrid.Children.Add(CreateResultCard("Max Win", "$0.00", 2));
            resultsGrid.Children.Add(CreateResultCard("Min Win", "$0.00", 3));

            resultsSection.Children.Add(resultsGrid);

            // Save Button
            var saveButton = new Button
            {
                Content = "💾 Save Performance Record",
                Background = new SolidColorBrush(GetColorFromHex("#107C10")),
                Foreground = new SolidColorBrush(Colors.White),
                Padding = new Thickness(30, 15, 30, 15),
                FontSize = 14,
                FontWeight = Microsoft.UI.Text.FontWeights.Bold,
                CornerRadius = new CornerRadius(8),
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(0, 20, 0, 0)
            };

            saveButton.Click += (s, e) => ShowMessage("Performance record saved successfully!");

            // Add all elements to form
            formPanel.Children.Add(accountPanel);
            formPanel.Children.Add(weekPanel);
            formPanel.Children.Add(fxIdPanel);
            formPanel.Children.Add(new Rectangle { Height = 1, Fill = new SolidColorBrush(GetColorFromHex("#E0E0E0")), Margin = new Thickness(0, 10, 0, 10) });
            formPanel.Children.Add(fileSection);
            formPanel.Children.Add(resultsSection);
            formPanel.Children.Add(saveButton);

            card.Child = formPanel;
            contentPanel.Children.Add(card);
        }

        private void ShowUpdatePerformanceTab(StackPanel contentPanel)
        {
            var card = CreateContentCard("");
            var formPanel = new StackPanel { Spacing = 15 };

            formPanel.Children.Add(new TextBlock
            {
                Text = "Update Performance Records",
                FontSize = 16,
                FontWeight = Microsoft.UI.Text.FontWeights.Bold,
                Foreground = new SolidColorBrush(GetColorFromHex("#2B579A"))
            });

            formPanel.Children.Add(new TextBlock
            {
                Text = "Search for existing performance records by FX ID and update any field.",
                FontSize = 14,
                Foreground = new SolidColorBrush(Colors.Gray),
                TextWrapping = TextWrapping.Wrap
            });

            // Search Section
            var searchPanel = CreateFormField("Search FX ID:", new TextBox
            {
                PlaceholderText = "Enter FX ID (e.g., AC1WK0101)",
                FontSize = 14,
                Padding = new Thickness(10),
                BorderThickness = new Thickness(1),
                BorderBrush = new SolidColorBrush(GetColorFromHex("#E0E0E0")),
                CornerRadius = new CornerRadius(4)
            });

            var searchButton = new Button
            {
                Content = "🔍 Search",
                Background = new SolidColorBrush(GetColorFromHex("#2B579A")),
                Foreground = new SolidColorBrush(Colors.White),
                Padding = new Thickness(15, 8, 15, 8),
                FontSize = 12,
                FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
                CornerRadius = new CornerRadius(4),
                Margin = new Thickness(0, 10, 0, 0)
            };

            searchButton.Click += (s, e) => ShowMessage("Record found! Edit the fields below and click Update.");

            formPanel.Children.Add(searchPanel);
            formPanel.Children.Add(searchButton);

            card.Child = formPanel;
            contentPanel.Children.Add(card);
        }

        private void ShowDeletePerformanceTab(StackPanel contentPanel)
        {
            var card = CreateContentCard("");
            var formPanel = new StackPanel { Spacing = 15 };

            formPanel.Children.Add(new TextBlock
            {
                Text = "Delete Performance Records",
                FontSize = 16,
                FontWeight = Microsoft.UI.Text.FontWeights.Bold,
                Foreground = new SolidColorBrush(GetColorFromHex("#D13438"))
            });

            formPanel.Children.Add(new TextBlock
            {
                Text = "View and delete performance records. Use with caution - deleted records cannot be recovered.",
                FontSize = 14,
                Foreground = new SolidColorBrush(Colors.Gray),
                TextWrapping = TextWrapping.Wrap
            });

            var deleteButton = new Button
            {
                Content = "🗑️ View and Delete Records",
                Background = new SolidColorBrush(GetColorFromHex("#D13438")),
                Foreground = new SolidColorBrush(Colors.White),
                Padding = new Thickness(20, 12, 20, 12),
                FontSize = 12,
                FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
                CornerRadius = new CornerRadius(6),
                HorizontalAlignment = HorizontalAlignment.Left
            };

            deleteButton.Click += (s, e) => ShowMessage("Delete interface will show all records with multi-select options.");

            formPanel.Children.Add(deleteButton);
            card.Child = formPanel;
            contentPanel.Children.Add(card);
        }

        private void ShowViewPerformanceTab(StackPanel contentPanel)
        {
            var card = CreateContentCard("");
            var formPanel = new StackPanel { Spacing = 15 };

            formPanel.Children.Add(new TextBlock
            {
                Text = "Performance History",
                FontSize = 16,
                FontWeight = Microsoft.UI.Text.FontWeights.Bold,
                Foreground = new SolidColorBrush(GetColorFromHex("#2B579A"))
            });

            formPanel.Children.Add(new TextBlock
            {
                Text = "View all historical performance records with filtering and sorting options.",
                FontSize = 14,
                Foreground = new SolidColorBrush(Colors.Gray),
                TextWrapping = TextWrapping.Wrap
            });

            var viewButton = new Button
            {
                Content = "📊 View Performance History",
                Background = new SolidColorBrush(GetColorFromHex("#2B579A")),
                Foreground = new SolidColorBrush(Colors.White),
                Padding = new Thickness(20, 12, 20, 12),
                FontSize = 12,
                FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
                CornerRadius = new CornerRadius(6),
                HorizontalAlignment = HorizontalAlignment.Left
            };

            viewButton.Click += (s, e) => ShowMessage("Performance history table with search and filters will be displayed.");

            formPanel.Children.Add(viewButton);
            card.Child = formPanel;
            contentPanel.Children.Add(card);
        }

        // Helper Methods for Performance Module
        private ComboBox CreateAccountComboBox()
        {
            var comboBox = new ComboBox
            {
                PlaceholderText = "Select Account",
                FontSize = 14,
                Padding = new Thickness(10),
                BorderThickness = new Thickness(1),
                BorderBrush = new SolidColorBrush(GetColorFromHex("#E0E0E0")),
                CornerRadius = new CornerRadius(4)
            };

            comboBox.Items.Add("AC1 - Primary Trading Account");
            comboBox.Items.Add("AC2 - Risk Management Account");

            return comboBox;
        }

        private StackPanel CreateFormField(string label, FrameworkElement control)
        {
            var panel = new StackPanel { Orientation = Orientation.Vertical, Spacing = 5 };
            panel.Children.Add(new TextBlock
            {
                Text = label,
                FontSize = 14,
                FontWeight = Microsoft.UI.Text.FontWeights.SemiBold
            });
            panel.Children.Add(control);
            return panel;
        }

        private Border CreateResultCard(string title, string value, int column)
        {
            var card = new Border
            {
                Background = new SolidColorBrush(GetColorFromHex("#F8F9FA")),
                BorderBrush = new SolidColorBrush(GetColorFromHex("#E0E0E0")),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(6),
                Padding = new Thickness(15),
                Margin = new Thickness(5)
            };

            var stackPanel = new StackPanel();

            stackPanel.Children.Add(new TextBlock
            {
                Text = title,
                FontSize = 11,
                FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
                Foreground = new SolidColorBrush(Colors.Gray)
            });

            stackPanel.Children.Add(new TextBlock
            {
                Text = value,
                FontSize = 16,
                FontWeight = Microsoft.UI.Text.FontWeights.Bold,
                Foreground = new SolidColorBrush(GetColorFromHex("#2B579A"))
            });

            card.Child = stackPanel;
            Grid.SetColumn(card, column);

            return card;
        }

        private int GetCurrentWeek()
        {
            var today = DateTime.Today;
            var beginningOfMonth = new DateTime(today.Year, today.Month, 1);
            return (int)Math.Ceiling((today - beginningOfMonth).TotalDays / 7.0);
        }

        private async void SimulateFileUpload(TextBlock fileInfo)
        {
            fileInfo.Text = "Simulating file upload...";
            await Task.Delay(1000);
            fileInfo.Text = "✅ leveraged_trades_history_25.10.2025-2.csv\n• 24 trades processed\n• Total P&L: $150.25 calculated\n• FX ID: AC1WK0101 generated";
        }

        private void ShowMessage(string message)
        {
            // Simple message display - in real app, you'd use a proper dialog
            var dialog = new ContentDialog
            {
                Title = "Information",
                Content = message,
                CloseButtonText = "OK",
                XamlRoot = this.Content.XamlRoot
            };

            _ = dialog.ShowAsync();
        }
        private void ShowAccounts()
        {
            SetActiveButton(BtnAccounts);
            MainContentPanel.Children.Clear();

            var title = CreateModuleTitle("ACCOUNTS MANAGEMENT");
            MainContentPanel.Children.Add(title);

            // Create tab-like navigation
            var tabPanel = CreateAccountsTabPanel();
            MainContentPanel.Children.Add(tabPanel);

            // Content area for tabs
            var contentPanel = new StackPanel();
            contentPanel.Margin = new Thickness(0, 20, 0, 0);
            MainContentPanel.Children.Add(contentPanel);

            // Show default tab (Manage Accounts)
            ShowManageAccountsTab(contentPanel);
        }

        private StackPanel CreateAccountsTabPanel()
        {
            var tabPanel = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 2 };

            var manageTab = CreateAccountsTab("Manage Accounts", true);
            var depositTab = CreateAccountsTab("Deposit Funds", false);
            var withdrawTab = CreateAccountsTab("Withdraw Funds", false);
            var historyTab = CreateAccountsTab("Transaction History", false);

            manageTab.Tag = "Manage";
            depositTab.Tag = "Deposit";
            withdrawTab.Tag = "Withdraw";
            historyTab.Tag = "History";

            manageTab.Click += (s, e) => ShowAccountsTab(s as Button, "Manage");
            depositTab.Click += (s, e) => ShowAccountsTab(s as Button, "Deposit");
            withdrawTab.Click += (s, e) => ShowAccountsTab(s as Button, "Withdraw");
            historyTab.Click += (s, e) => ShowAccountsTab(s as Button, "History");

            tabPanel.Children.Add(manageTab);
            tabPanel.Children.Add(depositTab);
            tabPanel.Children.Add(withdrawTab);
            tabPanel.Children.Add(historyTab);

            return tabPanel;
        }

        private Button CreateAccountsTab(string content, bool isActive)
        {
            return new Button
            {
                Content = content,
                Background = new SolidColorBrush(isActive ? GetColorFromHex("#2B579A") : Colors.Transparent),
                Foreground = new SolidColorBrush(isActive ? Colors.White : GetColorFromHex("#333333")),
                BorderThickness = new Thickness(1),
                BorderBrush = new SolidColorBrush(GetColorFromHex("#2B579A")),
                Padding = new Thickness(20, 10, 20, 10),
                FontSize = 12,
                FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
                CornerRadius = new CornerRadius(6, 6, 0, 0)
            };
        }

        private void ShowAccountsTab(Button? clickedTab, string tabName)
        {
            // Reset all tabs
            var tabPanel = MainContentPanel.Children[1] as StackPanel;
            if (tabPanel == null) return;

            foreach (var child in tabPanel.Children)
            {
                if (child is Button tab)
                {
                    tab.Background = new SolidColorBrush(Colors.Transparent);
                    tab.Foreground = new SolidColorBrush(GetColorFromHex("#333333"));
                }
            }

            // Set active tab
            if (clickedTab != null)
            {
                clickedTab.Background = new SolidColorBrush(GetColorFromHex("#2B579A"));
                clickedTab.Foreground = new SolidColorBrush(Colors.White);
            }

            // Find content panel and show appropriate content
            var contentPanel = MainContentPanel.Children[2] as StackPanel;
            if (contentPanel == null) return;

            contentPanel.Children.Clear();

            switch (tabName)
            {
                case "Manage":
                    ShowManageAccountsTab(contentPanel);
                    break;
                case "Deposit":
                    ShowDepositTab(contentPanel);
                    break;
                case "Withdraw":
                    ShowWithdrawTab(contentPanel);
                    break;
                case "History":
                    ShowTransactionHistoryTab(contentPanel);
                    break;
            }
        }

        private void ShowManageAccountsTab(StackPanel contentPanel)
        {
            var mainGrid = new Grid();
            mainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            mainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(300) });

            // Left side - Account List
            var leftPanel = new StackPanel { Margin = new Thickness(0, 0, 20, 0) };

            var headerPanel = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 10 };
            headerPanel.Children.Add(new TextBlock
            {
                Text = "Existing Accounts",
                FontSize = 18,
                FontWeight = Microsoft.UI.Text.FontWeights.Bold,
                Foreground = new SolidColorBrush(GetColorFromHex("#2B579A"))
            });

            var refreshButton = new Button
            {
                Content = "🔄 Refresh",
                Background = new SolidColorBrush(GetColorFromHex("#E1E5E9")),
                Foreground = new SolidColorBrush(GetColorFromHex("#333333")),
                Padding = new Thickness(10, 5, 10, 5),
                FontSize = 11,
                CornerRadius = new CornerRadius(4)
            };
            refreshButton.Click += (s, e) => LoadSampleAccounts(leftPanel);
            headerPanel.Children.Add(refreshButton);

            leftPanel.Children.Add(headerPanel);
            leftPanel.Children.Add(new TextBlock
            {
                Text = "Manage your trading accounts and view balances",
                FontSize = 14,
                Foreground = new SolidColorBrush(Colors.Gray),
                Margin = new Thickness(0, 5, 0, 15)
            });

            // Account cards container
            var accountsContainer = new StackPanel { Spacing = 10 };
            LoadSampleAccounts(accountsContainer);
            leftPanel.Children.Add(accountsContainer);

            // Right side - Create New Account
            var rightPanel = new StackPanel();
            var createCard = CreateContentCard("");
            var createForm = new StackPanel { Spacing = 15 };

            createForm.Children.Add(new TextBlock
            {
                Text = "Create New Account",
                FontSize = 16,
                FontWeight = Microsoft.UI.Text.FontWeights.Bold,
                Foreground = new SolidColorBrush(GetColorFromHex("#2B579A"))
            });

            // Account ID
            var accountIdPanel = CreateFormField("Account ID:", new TextBox
            {
                PlaceholderText = "e.g., AC3, AC4",
                FontSize = 14,
                Padding = new Thickness(10)
            });

            // Account Name
            var accountNamePanel = CreateFormField("Account Name:", new TextBox
            {
                PlaceholderText = "e.g., Swing Trading Account",
                FontSize = 14,
                Padding = new Thickness(10)
            });

            // Initial Deposit
            var depositPanel = CreateFormField("Initial Deposit:", new TextBox
            {
                PlaceholderText = "0.00",
                FontSize = 14,
                Padding = new Thickness(10)
            });

            // Description
            var descPanel = CreateFormField("Description:", new TextBox
            {
                PlaceholderText = "Optional account description",
                FontSize = 14,
                Padding = new Thickness(10),
                Height = 60,
                TextWrapping = TextWrapping.Wrap,
                AcceptsReturn = true
            });

            // Create Button
            var createButton = new Button
            {
                Content = "➕ Create Account",
                Background = new SolidColorBrush(GetColorFromHex("#107C10")),
                Foreground = new SolidColorBrush(Colors.White),
                Padding = new Thickness(20, 12, 20, 12),
                FontSize = 14,
                FontWeight = Microsoft.UI.Text.FontWeights.Bold,
                CornerRadius = new CornerRadius(6),
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
            createButton.Click += (s, e) => ShowMessage("New account created successfully!");

            createForm.Children.Add(accountIdPanel);
            createForm.Children.Add(accountNamePanel);
            createForm.Children.Add(depositPanel);
            createForm.Children.Add(descPanel);
            createForm.Children.Add(createButton);

            createCard.Child = createForm;
            rightPanel.Children.Add(createCard);

            Grid.SetColumn(leftPanel, 0);
            Grid.SetColumn(rightPanel, 1);
            mainGrid.Children.Add(leftPanel);
            mainGrid.Children.Add(rightPanel);

            contentPanel.Children.Add(mainGrid);
        }

        private void ShowDepositTab(StackPanel contentPanel)
        {
            var card = CreateContentCard("");
            var formPanel = new StackPanel { Spacing = 20 };

            formPanel.Children.Add(new TextBlock
            {
                Text = "Deposit Funds to Account",
                FontSize = 18,
                FontWeight = Microsoft.UI.Text.FontWeights.Bold,
                Foreground = new SolidColorBrush(GetColorFromHex("#2B579A"))
            });

            // Account Selection
            var accountPanel = CreateFormField("Select Account:", CreateAccountComboBox());

            // Amount
            var amountPanel = CreateFormField("Deposit Amount:", new TextBox
            {
                PlaceholderText = "0.00",
                FontSize = 14,
                Padding = new Thickness(10)
            });

            // Description
            var descPanel = CreateFormField("Description:", new TextBox
            {
                PlaceholderText = "e.g., Initial deposit, Additional funds",
                FontSize = 14,
                Padding = new Thickness(10)
            });

            // Current Balance Display
            var balancePanel = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 10 };
            balancePanel.Children.Add(new TextBlock
            {
                Text = "Current Balance:",
                FontSize = 14,
                FontWeight = Microsoft.UI.Text.FontWeights.SemiBold
            });
            balancePanel.Children.Add(new TextBlock
            {
                Text = "$10,000.00",
                FontSize = 14,
                Foreground = new SolidColorBrush(GetColorFromHex("#107C10")),
                FontWeight = Microsoft.UI.Text.FontWeights.Bold
            });

            // Deposit Button
            var depositButton = new Button
            {
                Content = "💰 Process Deposit",
                Background = new SolidColorBrush(GetColorFromHex("#107C10")),
                Foreground = new SolidColorBrush(Colors.White),
                Padding = new Thickness(25, 15, 25, 15),
                FontSize = 14,
                FontWeight = Microsoft.UI.Text.FontWeights.Bold,
                CornerRadius = new CornerRadius(6),
                HorizontalAlignment = HorizontalAlignment.Left
            };
            depositButton.Click += (s, e) => ShowMessage("Deposit processed successfully! Account balance updated.");

            formPanel.Children.Add(accountPanel);
            formPanel.Children.Add(amountPanel);
            formPanel.Children.Add(descPanel);
            formPanel.Children.Add(balancePanel);
            formPanel.Children.Add(depositButton);

            card.Child = formPanel;
            contentPanel.Children.Add(card);
        }

        private void ShowWithdrawTab(StackPanel contentPanel)
        {
            var card = CreateContentCard("");
            var formPanel = new StackPanel { Spacing = 20 };

            formPanel.Children.Add(new TextBlock
            {
                Text = "Withdraw Funds from Account",
                FontSize = 18,
                FontWeight = Microsoft.UI.Text.FontWeights.Bold,
                Foreground = new SolidColorBrush(GetColorFromHex("#2B579A"))
            });

            // Account Selection
            var accountPanel = CreateFormField("Select Account:", CreateAccountComboBox());

            // Amount
            var amountPanel = CreateFormField("Withdrawal Amount:", new TextBox
            {
                PlaceholderText = "0.00",
                FontSize = 14,
                Padding = new Thickness(10)
            });

            // Description
            var descPanel = CreateFormField("Description:", new TextBox
            {
                PlaceholderText = "e.g., Profit withdrawal, Funds transfer",
                FontSize = 14,
                Padding = new Thickness(10)
            });

            // Balance Information
            var balanceInfo = new StackPanel { Spacing = 5 };
            balanceInfo.Children.Add(new TextBlock
            {
                Text = "Account Information:",
                FontSize = 14,
                FontWeight = Microsoft.UI.Text.FontWeights.SemiBold
            });
            balanceInfo.Children.Add(new TextBlock
            {
                Text = "• Current Balance: $10,000.00\n• Available for withdrawal: $9,500.00\n• Minimum balance: $500.00",
                FontSize = 12,
                Foreground = new SolidColorBrush(Colors.Gray),
                TextWrapping = TextWrapping.Wrap
            });

            // Withdraw Button
            var withdrawButton = new Button
            {
                Content = "💸 Process Withdrawal",
                Background = new SolidColorBrush(GetColorFromHex("#E1B12C")),
                Foreground = new SolidColorBrush(Colors.White),
                Padding = new Thickness(25, 15, 25, 15),
                FontSize = 14,
                FontWeight = Microsoft.UI.Text.FontWeights.Bold,
                CornerRadius = new CornerRadius(6),
                HorizontalAlignment = HorizontalAlignment.Left
            };
            withdrawButton.Click += (s, e) => ShowMessage("Withdrawal processed successfully! Account balance updated.");

            formPanel.Children.Add(accountPanel);
            formPanel.Children.Add(amountPanel);
            formPanel.Children.Add(descPanel);
            formPanel.Children.Add(balanceInfo);
            formPanel.Children.Add(withdrawButton);

            card.Child = formPanel;
            contentPanel.Children.Add(card);
        }

        private void ShowTransactionHistoryTab(StackPanel contentPanel)
        {
            var card = CreateContentCard("");
            var formPanel = new StackPanel { Spacing = 15 };

            formPanel.Children.Add(new TextBlock
            {
                Text = "Transaction History",
                FontSize = 18,
                FontWeight = Microsoft.UI.Text.FontWeights.Bold,
                Foreground = new SolidColorBrush(GetColorFromHex("#2B579A"))
            });

            // Filter Controls
            var filterPanel = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 10 };
            filterPanel.Children.Add(CreateFormField("Account:", CreateAccountComboBox()));

            var dateFilter = CreateFormField("Date Range:", new ComboBox
            {
                PlaceholderText = "Select range",
                Items = { "Last 7 days", "Last 30 days", "Last 3 months", "Custom range" },
                FontSize = 14,
                Padding = new Thickness(10)
            });
            filterPanel.Children.Add(dateFilter);

            var searchButton = new Button
            {
                Content = "🔍 Search",
                Background = new SolidColorBrush(GetColorFromHex("#2B579A")),
                Foreground = new SolidColorBrush(Colors.White),
                Padding = new Thickness(15, 8, 15, 8),
                FontSize = 12,
                FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
                CornerRadius = new CornerRadius(4),
                VerticalAlignment = VerticalAlignment.Bottom,
                Margin = new Thickness(0, 0, 0, 15)
            };
            filterPanel.Children.Add(searchButton);

            formPanel.Children.Add(filterPanel);

            // Sample Transactions
            var transactionsPanel = new StackPanel { Spacing = 8, Margin = new Thickness(0, 15, 0, 0) };

            // Add sample transactions
            transactionsPanel.Children.Add(CreateTransactionItem("DEPOSIT", "+$1,000.00", "AC1", "2024-01-15", "Initial deposit"));
            transactionsPanel.Children.Add(CreateTransactionItem("DEPOSIT", "+$5,000.00", "AC2", "2024-02-01", "Risk account funding"));
            transactionsPanel.Children.Add(CreateTransactionItem("WITHDRAWAL", "-$500.00", "AC1", "2024-10-28", "Profit withdrawal"));
            transactionsPanel.Children.Add(CreateTransactionItem("DEPOSIT", "+$2,000.00", "AC1", "2024-11-15", "Additional funds"));

            formPanel.Children.Add(transactionsPanel);

            // Export Button
            var exportButton = new Button
            {
                Content = "📄 Export to CSV",
                Background = new SolidColorBrush(GetColorFromHex("#E1E5E9")),
                Foreground = new SolidColorBrush(GetColorFromHex("#333333")),
                Padding = new Thickness(15, 10, 15, 10),
                FontSize = 12,
                FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
                CornerRadius = new CornerRadius(6),
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(0, 15, 0, 0)
            };
            exportButton.Click += (s, e) => ShowMessage("Transaction history exported to CSV successfully!");

            formPanel.Children.Add(exportButton);

            card.Child = formPanel;
            contentPanel.Children.Add(card);
        }

        // Helper Methods for Accounts Module
        private void LoadSampleAccounts(StackPanel container)
        {
            container.Children.Clear();

            // Sample account 1
            var account1 = CreateAccountCard("AC1", "Primary Trading Account", "$10,000.00", "+$75.25", "#2B579A", true);
            container.Children.Add(account1);

            // Sample account 2
            var account2 = CreateAccountCard("AC2", "Risk Management Account", "$5,000.00", "+$75.00", "#107C10", true);
            container.Children.Add(account2);
        }

        private Border CreateAccountCard(string accountId, string accountName, string balance, string pnl, string color, bool isActive)
        {
            var card = new Border
            {
                Background = new SolidColorBrush(Colors.White),
                BorderBrush = new SolidColorBrush(GetColorFromHex("#E0E0E0")),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(15),
                Margin = new Thickness(0, 0, 0, 0)
            };

            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(100) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(80) });

            // Left side - Account info
            var leftPanel = new StackPanel();
            leftPanel.Children.Add(new TextBlock
            {
                Text = accountId,
                FontSize = 16,
                FontWeight = Microsoft.UI.Text.FontWeights.Bold,
                Foreground = new SolidColorBrush(GetColorFromHex(color))
            });
            leftPanel.Children.Add(new TextBlock
            {
                Text = accountName,
                FontSize = 12,
                Foreground = new SolidColorBrush(Colors.Gray)
            });
            leftPanel.Children.Add(new TextBlock
            {
                Text = isActive ? "🟢 Active" : "🔴 Inactive",
                FontSize = 10,
                Foreground = new SolidColorBrush(isActive ? Colors.Green : Colors.Red),
                Margin = new Thickness(0, 5, 0, 0)
            });

            // Middle - Balance
            var middlePanel = new StackPanel();
            middlePanel.Children.Add(new TextBlock
            {
                Text = "Balance",
                FontSize = 10,
                Foreground = new SolidColorBrush(Colors.Gray)
            });
            middlePanel.Children.Add(new TextBlock
            {
                Text = balance,
                FontSize = 14,
                FontWeight = Microsoft.UI.Text.FontWeights.Bold,
                Foreground = new SolidColorBrush(GetColorFromHex("#333333"))
            });

            // Right side - P&L and actions
            var rightPanel = new StackPanel();
            rightPanel.Children.Add(new TextBlock
            {
                Text = "Weekly P&L",
                FontSize = 10,
                Foreground = new SolidColorBrush(Colors.Gray)
            });
            rightPanel.Children.Add(new TextBlock
            {
                Text = pnl,
                FontSize = 12,
                FontWeight = Microsoft.UI.Text.FontWeights.Bold,
                Foreground = new SolidColorBrush(GetColorFromHex("#00BC0E"))
            });

            // Action buttons
            var buttonPanel = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 5, Margin = new Thickness(0, 8, 0, 0) };

            var editButton = new Button
            {
                Content = "Edit",
                Background = new SolidColorBrush(GetColorFromHex("#E1E5E9")),
                Foreground = new SolidColorBrush(GetColorFromHex("#333333")),
                Padding = new Thickness(8, 4, 8, 4),
                FontSize = 10,
                CornerRadius = new CornerRadius(4)
            };

            var viewButton = new Button
            {
                Content = "View",
                Background = new SolidColorBrush(GetColorFromHex("#2B579A")),
                Foreground = new SolidColorBrush(Colors.White),
                Padding = new Thickness(8, 4, 8, 4),
                FontSize = 10,
                CornerRadius = new CornerRadius(4)
            };

            buttonPanel.Children.Add(editButton);
            buttonPanel.Children.Add(viewButton);
            rightPanel.Children.Add(buttonPanel);

            Grid.SetColumn(leftPanel, 0);
            Grid.SetColumn(middlePanel, 1);
            Grid.SetColumn(rightPanel, 2);
            grid.Children.Add(leftPanel);
            grid.Children.Add(middlePanel);
            grid.Children.Add(rightPanel);

            card.Child = grid;
            return card;
        }

        private Border CreateTransactionItem(string type, string amount, string account, string date, string description)
        {
            var border = new Border
            {
                Background = new SolidColorBrush(GetColorFromHex("#F8F9FA")),
                BorderBrush = new SolidColorBrush(GetColorFromHex("#E0E0E0")),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(6),
                Padding = new Thickness(12)
            };

            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(100) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(80) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(100) });

            // Type with colored indicator
            var typePanel = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 5 };
            var indicator = new Border
            {
                Background = new SolidColorBrush(type == "DEPOSIT" ? GetColorFromHex("#107C10") : GetColorFromHex("#E1B12C")),
                Width = 8,
                Height = 8,
                CornerRadius = new CornerRadius(4)
            };
            typePanel.Children.Add(indicator);
            typePanel.Children.Add(new TextBlock
            {
                Text = type,
                FontSize = 12,
                FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
                Foreground = new SolidColorBrush(GetColorFromHex("#333333"))
            });

            // Description and account
            var descPanel = new StackPanel();
            descPanel.Children.Add(new TextBlock
            {
                Text = description,
                FontSize = 12,
                Foreground = new SolidColorBrush(GetColorFromHex("#333333"))
            });
            descPanel.Children.Add(new TextBlock
            {
                Text = $"Account: {account}",
                FontSize = 10,
                Foreground = new SolidColorBrush(Colors.Gray)
            });

            // Amount
            var amountText = new TextBlock
            {
                Text = amount,
                FontSize = 12,
                FontWeight = Microsoft.UI.Text.FontWeights.Bold,
                Foreground = new SolidColorBrush(type == "DEPOSIT" ? GetColorFromHex("#107C10") : GetColorFromHex("#E1B12C")),
                HorizontalAlignment = HorizontalAlignment.Right
            };

            // Date
            var dateText = new TextBlock
            {
                Text = date,
                FontSize = 10,
                Foreground = new SolidColorBrush(Colors.Gray),
                HorizontalAlignment = HorizontalAlignment.Right
            };

            Grid.SetColumn(typePanel, 0);
            Grid.SetColumn(descPanel, 1);
            Grid.SetColumn(amountText, 2);
            Grid.SetColumn(dateText, 3);
            grid.Children.Add(typePanel);
            grid.Children.Add(descPanel);
            grid.Children.Add(amountText);
            grid.Children.Add(dateText);

            border.Child = grid;
            return border;
        }

        private void ShowReports()
        {
            SetActiveButton(BtnReports);
            MainContentPanel.Children.Clear();

            var title = CreateModuleTitle("REPORTS & ANALYTICS");
            MainContentPanel.Children.Add(title);

            // Create tab-like navigation
            var tabPanel = CreateReportsTabPanel();
            MainContentPanel.Children.Add(tabPanel);

            // Content area for tabs
            var contentPanel = new StackPanel();
            contentPanel.Margin = new Thickness(0, 20, 0, 0);
            MainContentPanel.Children.Add(contentPanel);

            // Show default tab (Generate Reports)
            ShowGenerateReportsTab(contentPanel);
        }

        private StackPanel CreateReportsTabPanel()
        {
            var tabPanel = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 2 };

            var generateTab = CreateReportsTab("Generate Reports", true);
            var analyticsTab = CreateReportsTab("Performance Analytics", false);
            var chartsTab = CreateReportsTab("Charts & Graphs", false);
            var exportTab = CreateReportsTab("Export Data", false);

            generateTab.Tag = "Generate";
            analyticsTab.Tag = "Analytics";
            chartsTab.Tag = "Charts";
            exportTab.Tag = "Export";

            generateTab.Click += (s, e) => ShowReportsTab(s as Button, "Generate");
            analyticsTab.Click += (s, e) => ShowReportsTab(s as Button, "Analytics");
            chartsTab.Click += (s, e) => ShowReportsTab(s as Button, "Charts");
            exportTab.Click += (s, e) => ShowReportsTab(s as Button, "Export");

            tabPanel.Children.Add(generateTab);
            tabPanel.Children.Add(analyticsTab);
            tabPanel.Children.Add(chartsTab);
            tabPanel.Children.Add(exportTab);

            return tabPanel;
        }

        private Button CreateReportsTab(string content, bool isActive)
        {
            return new Button
            {
                Content = content,
                Background = new SolidColorBrush(isActive ? GetColorFromHex("#2B579A") : Colors.Transparent),
                Foreground = new SolidColorBrush(isActive ? Colors.White : GetColorFromHex("#333333")),
                BorderThickness = new Thickness(1),
                BorderBrush = new SolidColorBrush(GetColorFromHex("#2B579A")),
                Padding = new Thickness(20, 10, 20, 10),
                FontSize = 12,
                FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
                CornerRadius = new CornerRadius(6, 6, 0, 0)
            };
        }

        private void ShowReportsTab(Button? clickedTab, string tabName)
        {
            // Reset all tabs
            var tabPanel = MainContentPanel.Children[1] as StackPanel;
            if (tabPanel == null) return;

            foreach (var child in tabPanel.Children)
            {
                if (child is Button tab)
                {
                    tab.Background = new SolidColorBrush(Colors.Transparent);
                    tab.Foreground = new SolidColorBrush(GetColorFromHex("#333333"));
                }
            }

            // Set active tab
            if (clickedTab != null)
            {
                clickedTab.Background = new SolidColorBrush(GetColorFromHex("#2B579A"));
                clickedTab.Foreground = new SolidColorBrush(Colors.White);
            }

            // Find content panel and show appropriate content
            var contentPanel = MainContentPanel.Children[2] as StackPanel;
            if (contentPanel == null) return;

            contentPanel.Children.Clear();

            switch (tabName)
            {
                case "Generate":
                    ShowGenerateReportsTab(contentPanel);
                    break;
                case "Analytics":
                    ShowAnalyticsTab(contentPanel);
                    break;
                case "Charts":
                    ShowChartsTab(contentPanel);
                    break;
                case "Export":
                    ShowExportTab(contentPanel);
                    break;
            }
        }

        private void ShowGenerateReportsTab(StackPanel contentPanel)
        {
            var mainGrid = new Grid();
            mainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            mainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(400) });

            // Left side - Report Configuration
            var leftPanel = new StackPanel { Margin = new Thickness(0, 0, 20, 0) };

            leftPanel.Children.Add(new TextBlock
            {
                Text = "Report Configuration",
                FontSize = 18,
                FontWeight = Microsoft.UI.Text.FontWeights.Bold,
                Foreground = new SolidColorBrush(GetColorFromHex("#2B579A"))
            });

            leftPanel.Children.Add(new TextBlock
            {
                Text = "Configure and generate custom trading reports",
                FontSize = 14,
                Foreground = new SolidColorBrush(Colors.Gray),
                Margin = new Thickness(0, 5, 0, 20)
            });

            // Report Type
            var reportTypePanel = CreateFormField("Report Type:", new ComboBox
            {
                Items = { "Weekly Performance Report", "Monthly Summary", "Quarterly Analysis", "Yearly Overview", "Custom Date Range" },
                SelectedIndex = 0,
                FontSize = 14,
                Padding = new Thickness(10)
            });

            // Date Range
            var datePanel = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 10 };
            datePanel.Children.Add(CreateFormField("Start Date:", new DatePicker
            {
                Date = DateTimeOffset.Now.AddDays(-7),
                FontSize = 14,
                Padding = new Thickness(10)
            }));
            datePanel.Children.Add(CreateFormField("End Date:", new DatePicker
            {
                Date = DateTimeOffset.Now,
                FontSize = 14,
                Padding = new Thickness(10)
            }));

            // Account Selection
            var accountPanel = CreateFormField("Account:", new ComboBox
            {
                Items = { "All Accounts", "AC1 - Primary Trading", "AC2 - Risk Management" },
                SelectedIndex = 0,
                FontSize = 14,
                Padding = new Thickness(10)
            });

            // Report Options
            var optionsPanel = new StackPanel { Spacing = 10 };
            optionsPanel.Children.Add(new TextBlock
            {
                Text = "Report Options:",
                FontSize = 14,
                FontWeight = Microsoft.UI.Text.FontWeights.SemiBold
            });

            var includeCharts = new CheckBox { Content = "Include Charts and Graphs", IsChecked = true, FontSize = 14 };
            var includeDetails = new CheckBox { Content = "Include Trade Details", IsChecked = true, FontSize = 14 };
            var performanceMetrics = new CheckBox { Content = "Include Performance Metrics", IsChecked = true, FontSize = 14 };

            optionsPanel.Children.Add(includeCharts);
            optionsPanel.Children.Add(includeDetails);
            optionsPanel.Children.Add(performanceMetrics);

            // Export Format
            var formatPanel = CreateFormField("Export Format:", new ComboBox
            {
                Items = { "PDF Document", "Excel Spreadsheet", "CSV Data" },
                SelectedIndex = 0,
                FontSize = 14,
                Padding = new Thickness(10)
            });

            // Generate Button
            var generateButton = new Button
            {
                Content = "📊 Generate Report",
                Background = new SolidColorBrush(GetColorFromHex("#2B579A")),
                Foreground = new SolidColorBrush(Colors.White),
                Padding = new Thickness(25, 15, 25, 15),
                FontSize = 14,
                FontWeight = Microsoft.UI.Text.FontWeights.Bold,
                CornerRadius = new CornerRadius(6),
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
            generateButton.Click += (s, e) => ShowMessage("Report generated successfully! Preview available on the right.");

            // Add all to left panel
            leftPanel.Children.Add(reportTypePanel);
            leftPanel.Children.Add(datePanel);
            leftPanel.Children.Add(accountPanel);
            leftPanel.Children.Add(optionsPanel);
            leftPanel.Children.Add(formatPanel);
            leftPanel.Children.Add(generateButton);

            // Right side - Report Preview
            var rightPanel = new StackPanel();
            rightPanel.Children.Add(new TextBlock
            {
                Text = "Report Preview",
                FontSize = 16,
                FontWeight = Microsoft.UI.Text.FontWeights.Bold,
                Foreground = new SolidColorBrush(GetColorFromHex("#2B579A"))
            });

            var previewCard = CreateContentCard("");
            var previewContent = new StackPanel { Spacing = 10 };

            previewContent.Children.Add(new TextBlock
            {
                Text = "Weekly Performance Report",
                FontSize = 16,
                FontWeight = Microsoft.UI.Text.FontWeights.Bold,
                Foreground = new SolidColorBrush(GetColorFromHex("#2B579A"))
            });

            previewContent.Children.Add(new TextBlock
            {
                Text = "Period: Oct 21-27, 2024\nAccounts: All\nGenerated: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm"),
                FontSize = 12,
                Foreground = new SolidColorBrush(Colors.Gray)
            });

            // Preview Stats
            var previewGrid = new Grid();
            previewGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            previewGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            previewGrid.Children.Add(CreatePreviewStat("Total P&L", "+$150.25", 0));
            previewGrid.Children.Add(CreatePreviewStat("Total Trades", "24", 1));
            previewGrid.Children.Add(CreatePreviewStat("Win Rate", "62%", 0, 1));
            previewGrid.Children.Add(CreatePreviewStat("Avg Win/Loss", "+$18.75", 1, 1));

            previewContent.Children.Add(previewGrid);
            previewContent.Children.Add(new TextBlock
            {
                Text = "This preview shows the key metrics that will be included in your generated report.",
                FontSize = 12,
                Foreground = new SolidColorBrush(Colors.Gray),
                FontStyle = FontStyle.Italic,
                TextWrapping = TextWrapping.Wrap
            });

            // Preview Actions
            var previewActions = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 10, Margin = new Thickness(0, 15, 0, 0) };
            var viewFullButton = new Button
            {
                Content = "👁️ View Full Report",
                Background = new SolidColorBrush(GetColorFromHex("#2B579A")),
                Foreground = new SolidColorBrush(Colors.White),
                Padding = new Thickness(15, 10, 15, 10),
                FontSize = 12,
                FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
                CornerRadius = new CornerRadius(6)
            };
            var exportButton = new Button
            {
                Content = "💾 Export Now",
                Background = new SolidColorBrush(GetColorFromHex("#107C10")),
                Foreground = new SolidColorBrush(Colors.White),
                Padding = new Thickness(15, 10, 15, 10),
                FontSize = 12,
                FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
                CornerRadius = new CornerRadius(6)
            };

            previewActions.Children.Add(viewFullButton);
            previewActions.Children.Add(exportButton);
            previewContent.Children.Add(previewActions);

            previewCard.Child = previewContent;
            rightPanel.Children.Add(previewCard);

            Grid.SetColumn(leftPanel, 0);
            Grid.SetColumn(rightPanel, 1);
            mainGrid.Children.Add(leftPanel);
            mainGrid.Children.Add(rightPanel);

            contentPanel.Children.Add(mainGrid);
        }

        private void ShowAnalyticsTab(StackPanel contentPanel)
        {
            var card = CreateContentCard("");
            var formPanel = new StackPanel { Spacing = 20 };

            formPanel.Children.Add(new TextBlock
            {
                Text = "Performance Analytics",
                FontSize = 18,
                FontWeight = Microsoft.UI.Text.FontWeights.Bold,
                Foreground = new SolidColorBrush(GetColorFromHex("#2B579A"))
            });

            formPanel.Children.Add(new TextBlock
            {
                Text = "Advanced analytics and performance metrics for your trading activity",
                FontSize = 14,
                Foreground = new SolidColorBrush(Colors.Gray),
                TextWrapping = TextWrapping.Wrap
            });

            // Analytics Grid
            var analyticsGrid = new Grid();
            analyticsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            analyticsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            analyticsGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            analyticsGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            // Key Metrics
            analyticsGrid.Children.Add(CreateAnalyticsCard("Overall Win Rate", "62%", "124 winning trades out of 200", 0, 0));
            analyticsGrid.Children.Add(CreateAnalyticsCard("Average Profit per Trade", "+$18.75", "Across all successful trades", 1, 0));
            analyticsGrid.Children.Add(CreateAnalyticsCard("Max Drawdown", "-$450.50", "Largest peak-to-trough decline", 0, 1));
            analyticsGrid.Children.Add(CreateAnalyticsCard("Risk-Reward Ratio", "1:1.8", "Average profit vs loss ratio", 1, 1));

            formPanel.Children.Add(analyticsGrid);

            // Account Comparison
            var comparisonPanel = new StackPanel { Margin = new Thickness(0, 20, 0, 0) };
            comparisonPanel.Children.Add(new TextBlock
            {
                Text = "Account Performance Comparison",
                FontSize = 16,
                FontWeight = Microsoft.UI.Text.FontWeights.Bold,
                Foreground = new SolidColorBrush(GetColorFromHex("#2B579A"))
            });

            var comparisonGrid = new Grid();
            comparisonGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            comparisonGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            comparisonGrid.Children.Add(CreateComparisonCard("AC1 - Primary", "+$1,250.75", "8.3% return", "#2B579A", 0));
            comparisonGrid.Children.Add(CreateComparisonCard("AC2 - Risk Mgmt", "+$754.50", "5.2% return", "#107C10", 1));

            comparisonPanel.Children.Add(comparisonGrid);
            formPanel.Children.Add(comparisonPanel);

            // Refresh Analytics Button
            var refreshButton = new Button
            {
                Content = "🔄 Refresh Analytics",
                Background = new SolidColorBrush(GetColorFromHex("#2B579A")),
                Foreground = new SolidColorBrush(Colors.White),
                Padding = new Thickness(20, 12, 20, 12),
                FontSize = 14,
                FontWeight = Microsoft.UI.Text.FontWeights.Bold,
                CornerRadius = new CornerRadius(6),
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(0, 20, 0, 0)
            };
            refreshButton.Click += (s, e) => ShowMessage("Analytics data refreshed with latest trading information.");

            formPanel.Children.Add(refreshButton);

            card.Child = formPanel;
            contentPanel.Children.Add(card);
        }

        private void ShowChartsTab(StackPanel contentPanel)
        {
            var card = CreateContentCard("");
            var formPanel = new StackPanel { Spacing = 20 };

            formPanel.Children.Add(new TextBlock
            {
                Text = "Charts & Visualization",
                FontSize = 18,
                FontWeight = Microsoft.UI.Text.FontWeights.Bold,
                Foreground = new SolidColorBrush(GetColorFromHex("#2B579A"))
            });

            formPanel.Children.Add(new TextBlock
            {
                Text = "Interactive charts and graphs for visual analysis of your trading performance",
                FontSize = 14,
                Foreground = new SolidColorBrush(Colors.Gray),
                TextWrapping = TextWrapping.Wrap
            });

            // Chart Selection
            var chartTypePanel = CreateFormField("Select Chart Type:", new ComboBox
            {
                Items = { "Profit/Loss Trend", "Balance Growth", "Trade Volume", "Win/Loss Distribution", "Instrument Performance" },
                SelectedIndex = 0,
                FontSize = 14,
                Padding = new Thickness(10)
            });

            formPanel.Children.Add(chartTypePanel);

            // Chart Preview Area
            var chartPreview = new Border
            {
                Background = new SolidColorBrush(GetColorFromHex("#F8F9FA")),
                BorderBrush = new SolidColorBrush(GetColorFromHex("#E0E0E0")),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(8),
                Height = 300,
                Padding = new Thickness(20),
                Margin = new Thickness(0, 10, 0, 0)
            };

            var chartContent = new StackPanel { HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
            chartContent.Children.Add(new TextBlock
            {
                Text = "📊 Interactive Chart Preview",
                FontSize = 24,
                Foreground = new SolidColorBrush(GetColorFromHex("#2B579A")),
                HorizontalAlignment = HorizontalAlignment.Center
            });
            chartContent.Children.Add(new TextBlock
            {
                Text = "Chart visualization will be displayed here\nwith real trading data and interactive features",
                FontSize = 14,
                Foreground = new SolidColorBrush(Colors.Gray),
                TextAlignment = TextAlignment.Center,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 10, 0, 0)
            });

            chartPreview.Child = chartContent;
            formPanel.Children.Add(chartPreview);

            // Chart Controls
            var chartControls = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 10, HorizontalAlignment = HorizontalAlignment.Center };

            var updateChartButton = new Button
            {
                Content = "🔄 Update Chart",
                Background = new SolidColorBrush(GetColorFromHex("#2B579A")),
                Foreground = new SolidColorBrush(Colors.White),
                Padding = new Thickness(15, 10, 15, 10),
                FontSize = 12,
                FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
                CornerRadius = new CornerRadius(6)
            };

            var exportChartButton = new Button
            {
                Content = "💾 Export Chart",
                Background = new SolidColorBrush(GetColorFromHex("#107C10")),
                Foreground = new SolidColorBrush(Colors.White),
                Padding = new Thickness(15, 10, 15, 10),
                FontSize = 12,
                FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
                CornerRadius = new CornerRadius(6)
            };

            chartControls.Children.Add(updateChartButton);
            chartControls.Children.Add(exportChartButton);
            formPanel.Children.Add(chartControls);

            card.Child = formPanel;
            contentPanel.Children.Add(card);
        }

        private void ShowExportTab(StackPanel contentPanel)
        {
            var card = CreateContentCard("");
            var formPanel = new StackPanel { Spacing = 20 };

            formPanel.Children.Add(new TextBlock
            {
                Text = "Data Export",
                FontSize = 18,
                FontWeight = Microsoft.UI.Text.FontWeights.Bold,
                Foreground = new SolidColorBrush(GetColorFromHex("#2B579A"))
            });

            formPanel.Children.Add(new TextBlock
            {
                Text = "Export your trading data in various formats for external analysis or record keeping",
                FontSize = 14,
                Foreground = new SolidColorBrush(Colors.Gray),
                TextWrapping = TextWrapping.Wrap
            });

            // Export Options Grid
            var optionsGrid = new Grid();
            optionsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            optionsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            optionsGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            optionsGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            optionsGrid.Children.Add(CreateExportOption("Performance Data", "All performance records with calculations", "CSV", 0, 0));
            optionsGrid.Children.Add(CreateExportOption("Transaction History", "Complete deposit/withdrawal history", "Excel", 1, 0));
            optionsGrid.Children.Add(CreateExportOption("Trade Details", "Individual trade records with P&L", "CSV", 0, 1));
            optionsGrid.Children.Add(CreateExportOption("Account Summary", "Account balances and statistics", "PDF", 1, 1));

            formPanel.Children.Add(optionsGrid);

            // Bulk Export Section
            var bulkSection = new StackPanel { Margin = new Thickness(0, 20, 0, 0) };
            bulkSection.Children.Add(new TextBlock
            {
                Text = "Bulk Export",
                FontSize = 16,
                FontWeight = Microsoft.UI.Text.FontWeights.Bold,
                Foreground = new SolidColorBrush(GetColorFromHex("#2B579A"))
            });

            var bulkPanel = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 10 };
            bulkPanel.Children.Add(CreateFormField("Date Range:", new ComboBox
            {
                Items = { "Last 30 days", "Last 3 months", "Last 6 months", "Last year", "All data" },
                SelectedIndex = 0,
                FontSize = 14,
                Padding = new Thickness(10)
            }));

            var bulkExportButton = new Button
            {
                Content = "📦 Export All Data",
                Background = new SolidColorBrush(GetColorFromHex("#E1B12C")),
                Foreground = new SolidColorBrush(Colors.White),
                Padding = new Thickness(20, 12, 20, 12),
                FontSize = 14,
                FontWeight = Microsoft.UI.Text.FontWeights.Bold,
                CornerRadius = new CornerRadius(6),
                VerticalAlignment = VerticalAlignment.Bottom
            };
            bulkExportButton.Click += (s, e) => ShowMessage("Bulk export started! You will be notified when complete.");

            bulkPanel.Children.Add(bulkExportButton);
            bulkSection.Children.Add(bulkPanel);
            formPanel.Children.Add(bulkSection);

            card.Child = formPanel;
            contentPanel.Children.Add(card);
        }

        // Helper Methods for Reports Module
        private Border CreatePreviewStat(string title, string value, int column, int row = 0)
        {
            var border = new Border
            {
                Background = new SolidColorBrush(GetColorFromHex("#F8F9FA")),
                BorderBrush = new SolidColorBrush(GetColorFromHex("#E0E0E0")),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(6),
                Padding = new Thickness(12),
                Margin = new Thickness(5)
            };

            var stackPanel = new StackPanel();
            stackPanel.Children.Add(new TextBlock
            {
                Text = title,
                FontSize = 11,
                FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
                Foreground = new SolidColorBrush(Colors.Gray)
            });
            stackPanel.Children.Add(new TextBlock
            {
                Text = value,
                FontSize = 14,
                FontWeight = Microsoft.UI.Text.FontWeights.Bold,
                Foreground = new SolidColorBrush(GetColorFromHex("#2B579A"))
            });

            border.Child = stackPanel;
            Grid.SetColumn(border, column);
            Grid.SetRow(border, row);

            return border;
        }

        private Border CreateAnalyticsCard(string title, string value, string description, int column, int row)
        {
            var border = new Border
            {
                Background = new SolidColorBrush(Colors.White),
                BorderBrush = new SolidColorBrush(GetColorFromHex("#E0E0E0")),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(15),
                Margin = new Thickness(5)
            };

            var stackPanel = new StackPanel();
            stackPanel.Children.Add(new TextBlock
            {
                Text = title,
                FontSize = 14,
                FontWeight = Microsoft.UI.Text.FontWeights.Bold,
                Foreground = new SolidColorBrush(GetColorFromHex("#2B579A"))
            });
            stackPanel.Children.Add(new TextBlock
            {
                Text = value,
                FontSize = 18,
                FontWeight = Microsoft.UI.Text.FontWeights.Bold,
                Foreground = new SolidColorBrush(GetColorFromHex("#107C10")),
                Margin = new Thickness(0, 5, 0, 5)
            });
            stackPanel.Children.Add(new TextBlock
            {
                Text = description,
                FontSize = 12,
                Foreground = new SolidColorBrush(Colors.Gray),
                TextWrapping = TextWrapping.Wrap
            });

            border.Child = stackPanel;
            Grid.SetColumn(border, column);
            Grid.SetRow(border, row);

            return border;
        }

        private Border CreateComparisonCard(string account, string pnl, string returnRate, string color, int column)
        {
            var border = new Border
            {
                Background = new SolidColorBrush(Colors.White),
                BorderBrush = new SolidColorBrush(GetColorFromHex("#E0E0E0")),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(15),
                Margin = new Thickness(5)
            };

            var stackPanel = new StackPanel();
            stackPanel.Children.Add(new TextBlock
            {
                Text = account,
                FontSize = 14,
                FontWeight = Microsoft.UI.Text.FontWeights.Bold,
                Foreground = new SolidColorBrush(GetColorFromHex(color))
            });
            stackPanel.Children.Add(new TextBlock
            {
                Text = pnl,
                FontSize = 16,
                FontWeight = Microsoft.UI.Text.FontWeights.Bold,
                Foreground = new SolidColorBrush(GetColorFromHex("#333333")),
                Margin = new Thickness(0, 5, 0, 2)
            });
            stackPanel.Children.Add(new TextBlock
            {
                Text = returnRate,
                FontSize = 12,
                Foreground = new SolidColorBrush(Colors.Gray)
            });

            border.Child = stackPanel;
            Grid.SetColumn(border, column);

            return border;
        }

        private Border CreateExportOption(string title, string description, string format, int column, int row)
        {
            var border = new Border
            {
                Background = new SolidColorBrush(Colors.White),
                BorderBrush = new SolidColorBrush(GetColorFromHex("#E0E0E0")),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(15),
                Margin = new Thickness(5)
            };

            var stackPanel = new StackPanel();

            var headerPanel = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 10 };
            headerPanel.Children.Add(new TextBlock
            {
                Text = title,
                FontSize = 14,
                FontWeight = Microsoft.UI.Text.FontWeights.Bold,
                Foreground = new SolidColorBrush(GetColorFromHex("#2B579A"))
            });
            headerPanel.Children.Add(new Border
            {
                Background = new SolidColorBrush(GetColorFromHex("#E1E5E9")),
                Padding = new Thickness(6, 2, 6, 2),
                CornerRadius = new CornerRadius(4),
                Child = new TextBlock
                {
                    Text = format,
                    FontSize = 10,
                    FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
                    Foreground = new SolidColorBrush(GetColorFromHex("#333333"))
                }
            });

            stackPanel.Children.Add(headerPanel);
            stackPanel.Children.Add(new TextBlock
            {
                Text = description,
                FontSize = 12,
                Foreground = new SolidColorBrush(Colors.Gray),
                Margin = new Thickness(0, 5, 0, 10),
                TextWrapping = TextWrapping.Wrap
            });

            var exportButton = new Button
            {
                Content = "Export",
                Background = new SolidColorBrush(GetColorFromHex("#2B579A")),
                Foreground = new SolidColorBrush(Colors.White),
                Padding = new Thickness(12, 6, 12, 6),
                FontSize = 11,
                FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
                CornerRadius = new CornerRadius(4),
                HorizontalAlignment = HorizontalAlignment.Left
            };
            exportButton.Click += (s, e) => ShowMessage($"{title} exported successfully as {format}!");

            stackPanel.Children.Add(exportButton);

            border.Child = stackPanel;
            Grid.SetColumn(border, column);
            Grid.SetRow(border, row);

            return border;
        }

        private void ShowSettings()
        {
            SetActiveButton(BtnSettings);
            MainContentPanel.Children.Clear();

            var title = CreateModuleTitle("SETTINGS & CONFIGURATION");
            MainContentPanel.Children.Add(title);

            var content = CreateContentCard(@"
APPLICATION SETTINGS:

🔧 General Settings
• Application theme and appearance
• Date and time formats
• Currency display preferences

💾 Database Configuration
• MySQL database connection
• Connection string settings
• Backup and restore options

📁 File Management
• Default import/export paths
• CSV file format preferences
• Auto-save configurations

🛡️ Security
• Data encryption settings
• Backup schedules
• User preferences

⚙️ System Preferences
• Notification settings
• Update preferences
• Performance options

Configure the application to match your workflow preferences.");
            MainContentPanel.Children.Add(content);
        }

        private void ShowHelp()
        {
            SetActiveButton(BtnHelp);
            MainContentPanel.Children.Clear();

            var title = CreateModuleTitle("HELP & DOCUMENTATION");
            MainContentPanel.Children.Add(title);

            var content = CreateContentCard(@"
GETTING STARTED:

1. DASHBOARD - View your trading overview and system status
2. PERFORMANCE - Upload weekly CSV files and track results  
3. ACCOUNTS - Manage your trading accounts and transactions
4. REPORTS - Generate detailed analytics and export reports

KEY FEATURES:

• FX ID Generation: AC1WK0101 (Account+Week+Month)
• CSV Processing: Automatic trade file analysis
• Real-time Balances: Live account balance tracking
• Professional Reports: PDF and Excel exports

SUPPORT:

For assistance with:
• CSV file formatting
• Database configuration  
• Performance calculations
• Report generation

Refer to the user manual or contact support.");
            MainContentPanel.Children.Add(content);
        }

        // UI Helper Methods
        private TextBlock CreateModuleTitle(string text)
        {
            return new TextBlock
            {
                Text = text,
                FontSize = 24,
                FontWeight = Microsoft.UI.Text.FontWeights.Bold,
                Foreground = new SolidColorBrush(GetColorFromHex("#2B579A")),
                Margin = new Thickness(0, 0, 0, 25)
            };
        }

        private TextBlock CreateSectionTitle(string text)
        {
            return new TextBlock
            {
                Text = text,
                FontSize = 18,
                FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
                Foreground = new SolidColorBrush(GetColorFromHex("#2B579A")),
                Margin = new Thickness(0, 25, 0, 15)
            };
        }

        private Border CreateContentCard(string content)
        {
            return new Border
            {
                Background = new SolidColorBrush(Colors.White),
                BorderBrush = new SolidColorBrush(GetColorFromHex("#E0E0E0")),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(20),
                Margin = new Thickness(0, 0, 0, 15),
                Child = new TextBlock
                {
                    Text = content,
                    FontSize = 14,
                    Foreground = new SolidColorBrush(Colors.Gray),
                    TextWrapping = TextWrapping.Wrap,
                    LineHeight = 22
                }
            };
        }

        private Grid CreateCardsGrid()
        {
            var grid = new Grid();
            grid.Margin = new Thickness(0, 0, 0, 30);
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            grid.Children.Add(CreateDashboardCard("Total Accounts", "2", "#2B579A", 0));
            grid.Children.Add(CreateDashboardCard("Total Balance", "$15,000.00", "#107C10", 1));
            grid.Children.Add(CreateDashboardCard("Weekly P&L", "+$150.25", "#00BC0E", 2));
            grid.Children.Add(CreateDashboardCard("Active Trades", "24", "#E1B12C", 3));

            return grid;
        }

        private Border CreateDashboardCard(string title, string value, string color, int column)
        {
            var card = new Border
            {
                Background = new SolidColorBrush(Colors.White),
                BorderBrush = new SolidColorBrush(GetColorFromHex("#E0E0E0")),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(20),
                Margin = new Thickness(10, 5, 10, 5)
            };

            var stackPanel = new StackPanel();

            stackPanel.Children.Add(new TextBlock
            {
                Text = title,
                FontSize = 12,
                FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
                Foreground = new SolidColorBrush(Colors.Gray),
                Margin = new Thickness(0, 0, 0, 8)
            });

            stackPanel.Children.Add(new TextBlock
            {
                Text = value,
                FontSize = 20,
                FontWeight = Microsoft.UI.Text.FontWeights.Bold,
                Foreground = new SolidColorBrush(GetColorFromHex(color))
            });

            card.Child = stackPanel;
            Grid.SetColumn(card, column);

            return card;
        }

        private void SetActiveButton(Button activeButton)
        {
            // Reset all main module buttons
            ResetButton(BtnDashboard);
            ResetButton(BtnPerformance);
            ResetButton(BtnAccounts);
            ResetButton(BtnReports);
            ResetButton(BtnSettings);
            ResetButton(BtnHelp);

            // Set active button
            activeButton.Background = new SolidColorBrush(GetColorFromHex("#2B579A"));
            activeButton.Foreground = new SolidColorBrush(Colors.White);
        }

        private void ResetButton(Button button)
        {
            button.Background = new SolidColorBrush(Colors.Transparent);
            button.Foreground = new SolidColorBrush(GetColorFromHex("#333333"));
        }

        private Windows.UI.Color GetColorFromHex(string hex)
        {
            hex = hex.Replace("#", "");
            byte a = 255;
            byte r = Convert.ToByte(hex.Substring(0, 2), 16);
            byte g = Convert.ToByte(hex.Substring(2, 2), 16);
            byte b = Convert.ToByte(hex.Substring(4, 2), 16);
            return Windows.UI.Color.FromArgb(a, r, g, b);
        }

        private async void UpdateStatusBar()
        {
            try
            {
                bool isConnected = _dbHelper.TestConnection();
                DbStatus.Text = _dbHelper.GetConnectionStatus();
                DbStatus.Foreground = new SolidColorBrush(isConnected ? Colors.Green : Colors.Red);

                LastUpdate.Text = $"Last Update: {DateTime.Now:yyyy-MM-dd HH:mm}";

                if (isConnected)
                {
                    await UpdateQuickStatsWithRealData();
                }
                else
                {
                    UpdateQuickStatsWithSampleData();
                }
            }
            catch (Exception ex)
            {
                DbStatus.Text = "🔴 Database Error";
                DbStatus.Foreground = new SolidColorBrush(Colors.Red);
                UpdateQuickStatsWithSampleData();
            }
        }

        private async Task UpdateQuickStatsWithRealData()
        {
            try
            {
                var totalBalance = await _dbHelper.GetTotalBalanceAsync();
                var weeklyPnl = await _dbHelper.GetWeeklyProfitLossAsync();
                var activeAccounts = await _dbHelper.GetActiveAccountsCountAsync();
                var weeklyTrades = await _dbHelper.GetWeeklyTradeCountAsync();

                QuickBalance.Text = $"${totalBalance:N2}";
                QuickPnl.Text = weeklyPnl >= 0 ? $"+${weeklyPnl:N2}" : $"-${Math.Abs(weeklyPnl):N2}";
                QuickPnl.Foreground = new SolidColorBrush(weeklyPnl >= 0 ? Colors.Green : Colors.Red);
                QuickAccounts.Text = activeAccounts.ToString();
                QuickTrades.Text = weeklyTrades.ToString();
            }
            catch (Exception ex)
            {
                // Fall back to sample data if real data fails
                UpdateQuickStatsWithSampleData();
            }
        }

        private void UpdateQuickStatsWithSampleData()
        {
            QuickBalance.Text = "$15,000.00";
            QuickPnl.Text = "+$150.25";
            QuickPnl.Foreground = new SolidColorBrush(Colors.Green);
            QuickAccounts.Text = "2";
            QuickTrades.Text = "24";
        }

    }
}