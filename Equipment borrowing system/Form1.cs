using System.Drawing;
using System.Text.Json;

namespace Equipment_borrowing_system;

public partial class Form1 : Form
{
    private readonly TabControl portals = new();
    private readonly ComboBox userBox = new();
    private readonly DataGridView studentEquipmentGrid = Grid();
    private readonly DataGridView staffEquipmentGrid = Grid();
    private readonly DataGridView studentRequestGrid = Grid();
    private readonly DataGridView staffRequestGrid = Grid();
    private readonly DataGridView loanGrid = Grid();
    private readonly NumericUpDown quantityBox = new() { Minimum = 1, Maximum = 100, Value = 1, Width = 80 };
    private readonly DateTimePicker neededFrom = new() { Format = DateTimePickerFormat.Short, MinDate = DateTime.Today };
    private readonly DateTimePicker dueDate = new() { Format = DateTimePickerFormat.Short, MinDate = DateTime.Today.AddDays(1) };
    private AppData data = new();
    private readonly string dataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "EquipmentBorrowingSystem", "data.json");
    private bool IsStaff => portals.SelectedIndex == 1;
    private int? CurrentUserId => userBox.SelectedValue is int id ? id : null;

    public Form1()
    {
        Text = "Equipment Borrowing System";
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(1080, 680);
        ClientSize = new Size(1180, 760);
        BackColor = Color.FromArgb(244, 247, 251);
        LoadData();
        BuildInterface();
        RefreshAll();
    }

    private void LoadData()
    {
        try
        {
            if (File.Exists(dataPath)) data = JsonSerializer.Deserialize<AppData>(File.ReadAllText(dataPath)) ?? new AppData();
            if (data.Users.Count == 0 && data.Equipment.Count == 0)
            {
                data.Users.Add(new UserRecord { UserID = 1, FullName = "Demo Student", Email = "student@example.local", UserRole = "STUDENT" });
                data.Users.Add(new UserRecord { UserID = 2, FullName = "Demo Staff", Email = "staff@example.local", UserRole = "STAFF" });
                data.Equipment.Add(new EquipmentRecord { EquipmentID = 1, EquipmentName = "Laptop", Category = "Computer", TotalQuantity = 5, AvailableQuantity = 5 });
                data.Equipment.Add(new EquipmentRecord { EquipmentID = 2, EquipmentName = "Projector", Category = "Presentation", TotalQuantity = 2, AvailableQuantity = 2 });
                SaveData();
            }
        }
        catch (Exception ex) { MessageBox.Show($"Could not read local app data. {ex.Message}", "Local data", MessageBoxButtons.OK, MessageBoxIcon.Warning); data = new AppData(); }
    }

    private void SaveData()
    {
        string? directory = Path.GetDirectoryName(dataPath);
        if (!string.IsNullOrEmpty(directory)) Directory.CreateDirectory(directory);
        File.WriteAllText(dataPath, JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true }));
    }

    private void BuildInterface()
    {
        var header = new Panel { Dock = DockStyle.Top, Height = 104, Padding = new Padding(18), BackColor = Color.FromArgb(24, 52, 83) };
        header.Controls.Add(new Label { Text = "Equipment Borrowing System", ForeColor = Color.White, Font = new Font("Segoe UI", 18, FontStyle.Bold), AutoSize = true, Location = new Point(18, 10) });
        header.Controls.Add(new Label { Text = "Shared local data · staff and student pages", ForeColor = Color.FromArgb(204, 219, 235), Font = new Font("Segoe UI", 9), AutoSize = true, Location = new Point(20, 48) });
        userBox.DropDownStyle = ComboBoxStyle.DropDownList; userBox.SetBounds(820, 34, 220, 30);
        var addUser = Button("Add profile", 1050, 32, 105); addUser.Click += (_, _) => AddProfile();
        header.Controls.AddRange(new Control[] { userBox, addUser }); Controls.Add(header);

        portals.SetBounds(0, 104, ClientSize.Width, ClientSize.Height - 104); portals.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right; portals.Font = new Font("Segoe UI", 10);
        var studentPage = new TabPage("Student Page") { BackColor = Color.White };
        var staffPage = new TabPage("Staff Page") { BackColor = Color.White };
        portals.TabPages.AddRange(new[] { studentPage, staffPage });
        portals.SelectedIndexChanged += (_, _) => { LoadUsers(); RefreshAll(); };
        Controls.Add(portals); portals.BringToFront(); header.BringToFront();

        var studentTabs = new TabControl { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 9) };
        var studentCatalog = new TabPage("Equipment Catalog") { BackColor = Color.White };
        var studentRequests = new TabPage("My Requests") { BackColor = Color.White };
        studentTabs.TabPages.AddRange(new[] { studentCatalog, studentRequests }); studentPage.Controls.Add(studentTabs);
        studentEquipmentGrid.SetBounds(20, 20, 1090, 440); studentCatalog.Controls.Add(studentEquipmentGrid);
        var submit = Button("Request selected equipment", 20, 500, 220); submit.Click += (_, _) => SubmitRequest(); studentCatalog.Controls.Add(submit);
        studentCatalog.Controls.Add(new Label { Text = "Quantity", AutoSize = true, Location = new Point(255, 507) }); quantityBox.Location = new Point(315, 500); studentCatalog.Controls.Add(quantityBox);
        studentCatalog.Controls.Add(new Label { Text = "Needed from", AutoSize = true, Location = new Point(415, 507) }); neededFrom.Location = new Point(500, 500); studentCatalog.Controls.Add(neededFrom);
        studentCatalog.Controls.Add(new Label { Text = "Due date", AutoSize = true, Location = new Point(620, 507) }); dueDate.Location = new Point(680, 500); studentCatalog.Controls.Add(dueDate);
        studentRequestGrid.SetBounds(20, 20, 1090, 490); studentRequests.Controls.Add(studentRequestGrid);

        var staffTabs = new TabControl { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 9) };
        var staffInventory = new TabPage("Equipment Inventory") { BackColor = Color.White };
        var staffRequests = new TabPage("Request Review") { BackColor = Color.White };
        var staffLoans = new TabPage("Loans") { BackColor = Color.White };
        staffTabs.TabPages.AddRange(new[] { staffInventory, staffRequests, staffLoans }); staffPage.Controls.Add(staffTabs);
        staffEquipmentGrid.SetBounds(20, 20, 1090, 440); staffInventory.Controls.Add(staffEquipmentGrid);
        var addEquipment = Button("Add equipment", 20, 500, 150); addEquipment.Click += (_, _) => AddEquipment();
        var editEquipment = Button("Edit selected", 180, 500, 150); editEquipment.Click += (_, _) => EditEquipment(); staffInventory.Controls.AddRange(new Control[] { addEquipment, editEquipment });
        staffRequestGrid.SetBounds(20, 20, 1090, 440); staffRequests.Controls.Add(staffRequestGrid);
        var approve = Button("Approve", 20, 500, 120); approve.Click += (_, _) => ReviewRequest(true);
        var reject = Button("Reject", 150, 500, 120); reject.Click += (_, _) => ReviewRequest(false); staffRequests.Controls.AddRange(new Control[] { approve, reject });
        loanGrid.SetBounds(20, 20, 1090, 440); staffLoans.Controls.Add(loanGrid);
        var checkout = Button("Check out approved request", 20, 500, 220); checkout.Click += (_, _) => Checkout();
        var returnItem = Button("Record return", 250, 500, 150); returnItem.Click += (_, _) => ReturnLoan(); staffLoans.Controls.AddRange(new Control[] { checkout, returnItem });
        LoadUsers();
    }
    private static DataGridView Grid() => new() { ReadOnly = true, AllowUserToAddRows = false, AllowUserToDeleteRows = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, SelectionMode = DataGridViewSelectionMode.FullRowSelect, MultiSelect = false, BackgroundColor = Color.White, RowHeadersVisible = false, BorderStyle = BorderStyle.FixedSingle };
    private static Button Button(string text, int x, int y, int width) => new() { Text = text, Height = 34, Width = width, Location = new Point(x, y) };

    private void LoadUsers()
    {
        var users = data.Users.Where(u => u.IsActive && u.UserRole == (IsStaff ? "STAFF" : "STUDENT")).OrderBy(u => u.FullName).ToList();
        userBox.DataSource = null; userBox.DataSource = users; userBox.DisplayMember = nameof(UserRecord.FullName); userBox.ValueMember = nameof(UserRecord.UserID);
    }

    private void RefreshAll()
    {
        studentEquipmentGrid.DataSource = data.Equipment.Where(e => e.IsActive).OrderBy(e => e.EquipmentName).Select(e => new { e.EquipmentID, e.EquipmentName, e.Category, e.TotalQuantity, e.AvailableQuantity }).ToList();
        staffEquipmentGrid.DataSource = data.Equipment.Where(e => e.IsActive).OrderBy(e => e.EquipmentName).Select(e => new { e.EquipmentID, e.EquipmentName, e.Category, e.TotalQuantity, e.AvailableQuantity }).ToList();
        var myRequests = data.Requests.AsEnumerable();
        if (CurrentUserId is int studentId) myRequests = myRequests.Where(r => r.StudentID == studentId);
        studentRequestGrid.DataSource = myRequests.OrderByDescending(r => r.SubmittedAt).Select(r => new { r.RequestID, r.SubmittedAt, r.NeededFrom, r.DueDate, r.RequestStatus, Items = string.Join(", ", r.Items.Select(i => $"{data.Equipment.FirstOrDefault(e => e.EquipmentID == i.EquipmentID)?.EquipmentName ?? "Item"} x{i.Quantity}")), r.DecisionNote }).ToList();
        staffRequestGrid.DataSource = data.Requests.OrderByDescending(r => r.SubmittedAt).Select(r => new { r.RequestID, Student = data.Users.FirstOrDefault(u => u.UserID == r.StudentID)?.FullName ?? "Unknown", r.SubmittedAt, r.NeededFrom, r.DueDate, r.RequestStatus, Items = string.Join(", ", r.Items.Select(i => $"{data.Equipment.FirstOrDefault(e => e.EquipmentID == i.EquipmentID)?.EquipmentName ?? "Item"} x{i.Quantity}")), r.DecisionNote }).ToList();
        loanGrid.DataSource = data.Requests.Where(r => new[] { "APPROVED", "CHECKED_OUT", "RETURNED" }.Contains(r.RequestStatus)).OrderBy(r => r.DueDate).Select(r => { var loan = data.Loans.FirstOrDefault(l => l.RequestID == r.RequestID); return new { r.RequestID, Student = data.Users.FirstOrDefault(u => u.UserID == r.StudentID)?.FullName ?? "Unknown", r.RequestStatus, r.NeededFrom, r.DueDate, loan?.CheckoutAt, loan?.ReturnedAt, LoanStatus = r.RequestStatus == "APPROVED" ? "Ready for checkout" : r.RequestStatus == "CHECKED_OUT" && r.DueDate < DateTime.Today ? "OVERDUE" : r.RequestStatus }; }).ToList();
    }
    private void AddProfile()
    {
        string? name = Prompt("Full name:", "Add profile"); if (string.IsNullOrWhiteSpace(name)) return;
        string? email = Prompt("Email:", "Add profile"); if (string.IsNullOrWhiteSpace(email)) return;
        if (data.Users.Any(u => string.Equals(u.Email, email.Trim(), StringComparison.OrdinalIgnoreCase))) { MessageBox.Show("That email is already in use."); return; }
        data.Users.Add(new UserRecord { UserID = data.Users.Select(u => u.UserID).DefaultIfEmpty(0).Max() + 1, FullName = name.Trim(), Email = email.Trim(), UserRole = IsStaff ? "STAFF" : "STUDENT" }); SaveData(); LoadUsers(); RefreshAll();
    }

    private void AddEquipment()
    {
        if (!IsStaff) return;
        string? name = Prompt("Equipment name:", "Add equipment"); if (string.IsNullOrWhiteSpace(name)) return;
        string? category = Prompt("Category (optional):", "Add equipment") ?? "";
        string? qtyText = Prompt("Total quantity:", "Add equipment"); if (!int.TryParse(qtyText, out int qty) || qty < 0) { MessageBox.Show("Enter a whole number 0 or greater."); return; }
        data.Equipment.Add(new EquipmentRecord { EquipmentID = data.Equipment.Select(e => e.EquipmentID).DefaultIfEmpty(0).Max() + 1, EquipmentName = name.Trim(), Category = category.Trim(), TotalQuantity = qty, AvailableQuantity = qty }); SaveData(); RefreshAll();
    }

    private void EditEquipment()
    {
        if (!IsStaff || staffEquipmentGrid.CurrentRow == null) return;
        int id = Convert.ToInt32(staffEquipmentGrid.CurrentRow.Cells[nameof(EquipmentRecord.EquipmentID)].Value); var item = data.Equipment.First(e => e.EquipmentID == id);
        string? name = Prompt("Equipment name:", "Edit equipment", item.EquipmentName); if (string.IsNullOrWhiteSpace(name)) return;
        string? totalText = Prompt("New total quantity:", "Edit equipment", item.TotalQuantity.ToString()); if (!int.TryParse(totalText, out int total) || total < item.TotalQuantity - item.AvailableQuantity) { MessageBox.Show("Total cannot be lower than the number currently on loan."); return; }
        item.EquipmentName = name.Trim(); item.AvailableQuantity += total - item.TotalQuantity; item.TotalQuantity = total; SaveData(); RefreshAll();
    }

    private void SubmitRequest()
    {
        if (IsStaff || CurrentUserId is not int studentId || studentEquipmentGrid.CurrentRow == null) { MessageBox.Show("Choose a Student profile and select equipment first."); return; }
        int itemId = Convert.ToInt32(studentEquipmentGrid.CurrentRow.Cells[nameof(EquipmentRecord.EquipmentID)].Value); var equipment = data.Equipment.First(e => e.EquipmentID == itemId);
        if (dueDate.Value.Date < neededFrom.Value.Date) { MessageBox.Show("Due date must be on or after the needed-from date."); return; }
        if (quantityBox.Value > equipment.AvailableQuantity) { MessageBox.Show("There is not enough available equipment for that quantity."); return; }
        int requestId = data.Requests.Select(r => r.RequestID).DefaultIfEmpty(0).Max() + 1;
        data.Requests.Add(new RequestRecord { RequestID = requestId, StudentID = studentId, SubmittedAt = DateTime.Now, NeededFrom = neededFrom.Value.Date, DueDate = dueDate.Value.Date, RequestStatus = "PENDING", Items = new List<RequestItemRecord> { new() { EquipmentID = itemId, Quantity = (int)quantityBox.Value } } });
        SaveData(); RefreshAll(); MessageBox.Show($"Request #{requestId} submitted for staff review.");
    }

    private void ReviewRequest(bool approve)
    {
        if (!IsStaff || CurrentUserId is not int staffId || staffRequestGrid.CurrentRow == null) { MessageBox.Show("Choose a Staff profile and select a request."); return; }
        int id = Convert.ToInt32(staffRequestGrid.CurrentRow.Cells[nameof(RequestRecord.RequestID)].Value); var request = data.Requests.First(r => r.RequestID == id);
        if (request.RequestStatus != "PENDING") { MessageBox.Show("This request is no longer pending."); return; }
        string? note = Prompt("Optional decision note:", approve ? "Approve request" : "Reject request");
        if (approve && request.Items.Any(i => data.Equipment.First(e => e.EquipmentID == i.EquipmentID).AvailableQuantity < i.Quantity)) { MessageBox.Show("Not enough equipment is available. The request was not changed."); return; }
        if (approve) foreach (var item in request.Items) data.Equipment.First(e => e.EquipmentID == item.EquipmentID).AvailableQuantity -= item.Quantity;
        request.RequestStatus = approve ? "APPROVED" : "REJECTED"; request.ReviewedBy = staffId; request.ReviewedAt = DateTime.Now; request.DecisionNote = note; SaveData(); RefreshAll();
    }

    private void Checkout()
    {
        if (!IsStaff || CurrentUserId is not int staffId || loanGrid.CurrentRow == null) { MessageBox.Show("Choose a Staff profile and an approved request."); return; }
        int id = Convert.ToInt32(loanGrid.CurrentRow.Cells[nameof(RequestRecord.RequestID)].Value); var request = data.Requests.First(r => r.RequestID == id);
        if (request.RequestStatus != "APPROVED") { MessageBox.Show("Select a request with APPROVED status."); return; }
        data.Loans.Add(new LoanRecord { RequestID = id, CheckedOutBy = staffId, CheckoutAt = DateTime.Now }); request.RequestStatus = "CHECKED_OUT"; SaveData(); RefreshAll();
    }

    private void ReturnLoan()
    {
        if (!IsStaff || loanGrid.CurrentRow == null) { MessageBox.Show("Choose a Staff profile and a checked-out request."); return; }
        int id = Convert.ToInt32(loanGrid.CurrentRow.Cells[nameof(RequestRecord.RequestID)].Value); var request = data.Requests.First(r => r.RequestID == id);
        if (request.RequestStatus != "CHECKED_OUT") { MessageBox.Show("Select a request that is currently checked out."); return; }
        foreach (var item in request.Items) data.Equipment.First(e => e.EquipmentID == item.EquipmentID).AvailableQuantity += item.Quantity;
        var loan = data.Loans.FirstOrDefault(l => l.RequestID == id); if (loan != null) loan.ReturnedAt = DateTime.Now;
        request.RequestStatus = "RETURNED"; SaveData(); RefreshAll();
    }

    private static string? Prompt(string message, string title, string initial = "")
    {
        using var form = new Form { Text = title, FormBorderStyle = FormBorderStyle.FixedDialog, StartPosition = FormStartPosition.CenterParent, Width = 410, Height = 155, MinimizeBox = false, MaximizeBox = false };
        var label = new Label { Text = message, AutoSize = true, Left = 12, Top = 14 }; var box = new TextBox { Text = initial, Left = 12, Top = 40, Width = 370 };
        var ok = new Button { Text = "OK", DialogResult = DialogResult.OK, Left = 220, Top = 76, Width = 75 }; var cancel = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel, Left = 307, Top = 76, Width = 75 };
        form.Controls.AddRange(new Control[] { label, box, ok, cancel }); form.AcceptButton = ok; form.CancelButton = cancel; return form.ShowDialog() == DialogResult.OK ? box.Text : null;
    }
}

internal sealed class AppData
{
    public List<UserRecord> Users { get; set; } = new();
    public List<EquipmentRecord> Equipment { get; set; } = new();
    public List<RequestRecord> Requests { get; set; } = new();
    public List<LoanRecord> Loans { get; set; } = new();
}
internal sealed class UserRecord { public int UserID { get; set; } public string FullName { get; set; } = ""; public string Email { get; set; } = ""; public string UserRole { get; set; } = "STUDENT"; public bool IsActive { get; set; } = true; }
internal sealed class EquipmentRecord { public int EquipmentID { get; set; } public string EquipmentName { get; set; } = ""; public string Category { get; set; } = ""; public int TotalQuantity { get; set; } public int AvailableQuantity { get; set; } public bool IsActive { get; set; } = true; }
internal sealed class RequestRecord { public int RequestID { get; set; } public int StudentID { get; set; } public DateTime SubmittedAt { get; set; } public DateTime NeededFrom { get; set; } public DateTime DueDate { get; set; } public string RequestStatus { get; set; } = "PENDING"; public int? ReviewedBy { get; set; } public DateTime? ReviewedAt { get; set; } public string? DecisionNote { get; set; } public List<RequestItemRecord> Items { get; set; } = new(); }
internal sealed class RequestItemRecord { public int EquipmentID { get; set; } public int Quantity { get; set; } }
internal sealed class LoanRecord { public int RequestID { get; set; } public int CheckedOutBy { get; set; } public DateTime CheckoutAt { get; set; } public DateTime? ReturnedAt { get; set; } }






