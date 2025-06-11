<!-- Improved compatibility of back to top link: See: https://github.com/othneildrew/Best-README-Template/pull/73 -->
<a id="readme-top"></a>

<!-- TABLE OF CONTENTS -->
<details>
  <summary>Table of Contents</summary>
  <ol>
    <li>
      <a href="#about-the-project">About The Project</a>
      <ul>
        <li><a href="#built-with">Built With</a></li>
        <li><a href="#Features">Features</a></li>
        <li><a href="#Architecture">Architecture</a></li>
      </ul>
    </li>
    <li>
      <a href="#getting-started">Getting Started</a>
      <ul>
        <li><a href="#prerequisites">Prerequisites</a></li>
        <li><a href="#installation">Installation</a></li>
      </ul>
    </li>
    <li><a href="#license">License</a></li>
    <li><a href="#contact">Contact</a></li>
  </ol>
</details>


<!-- ABOUT THE PROJECT -->
## About The Project

This project is a Kanban Board application built with WPF and C#. It allows users to visually manage tasks using columns (such as "To Do", "In Progress", "Review", and "Done") and cards representing individual tasks. Users can add, edit, move, and delete cards, as well as add or remove columns. Each card tracks its own history of changes, including edits and status transitions. The board state can be saved to and loaded from a JSON file, enabling persistent task management. The application follows the MVVM (Model-View-ViewModel) pattern for maintainability and separation of concerns.


<p align="right">(<a href="#readme-top">back to top</a>)</p>


### Built With

- **.NET 9** – modern, high-performance application framework
- **WPF (Windows Presentation Foundation)** – for rich desktop UI
- **XAML** – declarative UI markup for WPF
- **MVVM Pattern** – for separation of concerns and maintainable code
- **Custom Controls** – for reusable UI components (e.g., PlaceHolderTextBox)
- **System.Text.Json** – for fast, built-in JSON serialization
- **ObservableCollection** – for dynamic data binding in UI

<p align="right">(<a href="#readme-top">back to top</a>)</p>


<!-- Features Overview -->
### Features

- Create, edit, and delete Kanban cards with title, owner, description, urgency, status, due date, and comments
- Add, rename, move, and delete columns dynamically
- Drag and drop cards between columns for easy workflow management
- Reorder columns via drag and drop
- Card history tracking: automatically logs changes to card properties (title, owner, description, urgency, status, due date, comments)
- Save and load the entire board to/from JSON files for persistence
- Responsive and modern WPF UI with custom controls (e.g., placeholder text boxes)
- Visual indicators for card urgency and due dates
- Double-click to edit cards or column titles inline
- Confirmation dialogs for destructive actions (e.g., deleting cards or columns)

<p align="right">(<a href="#readme-top">back to top</a>)</p>

<!-- GETTING STARTED -->
## Getting Started

To get a local copy up and running follow these simple example steps.

### Prerequisites

Docker Desktop & Docker Compose installed locally.

### Installation

Below is an example of how you can instruct your audience on installing and setting up your app.

1. Clone the repo
   ```sh
   git clone https://github.com/DrLeXXan/KanbanBoardApp.git
   ```
2. Run the application (locally via Visual Studio)
   - Open the solution in Visual Studio 2022 or later.
   -  Restore NuGet packages if prompted.
   -  Set KanbanBoardApp as the startup project.
   -  Press F5 or click Start to launch the app.

<p align="right">(<a href="#readme-top">back to top</a>)</p>


<!-- LICENSE -->
## License

Distributed under the MIT License. See `LICENSE.txt` for more information.

<p align="right">(<a href="#readme-top">back to top</a>)</p>



<!-- CONTACT -->
## Contact

LinkedIn - [@lennartboehm](https://www.linkedin.com/in/lennartboehm)

Project Link: [https://github.com/DrLeXXan/KanbanBoardApp](https://github.com/DrLeXXan/KanbanBoardApp)

<p align="right">(<a href="#readme-top">back to top</a>)</p>