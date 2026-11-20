# Equipment Borrowing System

A school project for managing equipment loans and helping students see what is available.

## First version

A Windows Forms app with separate Student and Staff pages. Both pages use the same data saved locally on the computer, so equipment added by staff appears in the student catalog and student requests appear in the staff review page.

- **Student:** browse equipment, submit a request, and view personal request status.
- **Staff:** add or edit equipment, approve or reject requests, and record checkout and return.
- **Storage:** local JSON file at `%LocalAppData%\EquipmentBorrowingSystem\data.json`; SQL Server is not required.

## Run

Open `Equipment borrowing system.slnx` in Visual Studio and run the app. On first launch, it creates sample Student and Staff profiles and Laptop and Projector items. The role pages are for a classroom demonstration and are not password-protected. Each request currently contains one equipment type.

See `Equipment borrowing system/README.md` for more details.
