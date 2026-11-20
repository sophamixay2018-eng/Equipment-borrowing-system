# Equipment Borrowing System — standalone app

This Windows desktop app does not need SQL Server. It saves data locally on this computer at `%LocalAppData%\EquipmentBorrowingSystem\data.json`.

## Pages and shared information

- **Student Page:** browse the equipment catalog, submit a request, and view personal requests.
- **Staff Page:** add or edit equipment, review student requests, and record checkout and return.
- Both pages use the same local data file. Equipment added by staff is available in the student catalog, and student requests appear on the staff review page.

## Run

Open the solution in Visual Studio and run the project. On first launch, the app creates sample Student and Staff profiles and sample Laptop and Projector equipment. Switch between pages using the tabs at the top; select the matching profile beside the page header. Use **Add profile** to add people.

The role pages are for a classroom demonstration and are not password-protected. A request in this first version contains one equipment type.
