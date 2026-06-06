# Food Maker App

A comprehensive mobile application built with .NET MAUI that allows users to search for, create, and share recipes while interacting with a vibrant community. Designed with both beginners and advanced users in mind, the app provides free recipes—even if you have only one ingredient at home—and offers a robust community where you can publish your own recipes, add friends, and comment on recipes. With a focus on exceptional UI/UX design, accessibility, and native hardware integration, this project exemplifies best practices and high-quality development.

## Table of Contents

- [Project Description](#project-description)
- [Features](#features)
- [Installation and Setup](#installation-and-setup)
- [Usage](#usage)
- [Page Behaviors and Rules](#page-behaviors-and-rules)
- [Firebase Integration and Data Handling](#firebase-integration-and-data-handling)
- [Accessibility](#accessibility)
- [Mobile Hardware Integration](#mobile-hardware-integration)
- [Code Structure and Quality](#code-structure-and-quality)
- [GitHub Repository and Commit History](#github-repository-and-commit-history)
- [License](#license)
- [Contributors](#contributors)
- [References](#references)

## Project Description

Food Maker App is designed for cooking enthusiasts who may have limited ingredients at home yet still want to prepare delicious meals. This application provides a full-featured platform to:

- **Browse Recipes:**  
  Explore a dynamic community feed where you can find recipes tailored for various ingredient availability.

- **Create and Manage Recipes:**  
  Submit your own recipes with detailed instructions, ingredient lists, and photos—even if you only have one ingredient.

- **Community Interaction:**  
  Engage with a vibrant community by publishing your recipes, adding friends, and commenting on others' recipes.

- **Hardware Integration:**  
  Utilize native mobile features such as the camera, flashlight, location services, haptic feedback, and text-to-speech for an enriched user experience.

- **Dynamic Theming and Accessibility:**  
  Customize the interface with dark/light modes and adjustable text sizes, while comprehensive accessibility enhancements ensure the app is usable for everyone.

## Features

- **Dynamic Theming & Responsive UI:**  
  Uses dynamic resources for colors and fonts to seamlessly support dark/light modes and adjustable text sizes, providing an outstanding user experience across all devices.

- **Accessibility Enhancements:**  
  Comprehensive use of SemanticProperties (Description, Hint, HeadingLevel) across every page and custom control ensures that the app meets WCAG guidelines and is fully accessible to users with disabilities.

- **User Authentication:**  
  Secure Firebase Authentication for safe and reliable user login and registration.

- **Recipe Management & CSV Parsing:**  
  Submit, edit, and delete recipes with detailed instructions and ingredient lists. A custom CSV parser loads sample recipe data, ensuring even users with minimal ingredients have access to recipes.

- **Community Interaction:**  
  Enjoy a dynamic community feed that updates in real time. Users can view, comment, and interact with recipes, enhancing the social experience.

- **Device Compatibility:**  
  The app is fully compatible with Pixel Tablet and Pixel 7 Pro, both running Android API 35.

- **Mobile Hardware Integration:**  
  - **Camera:** Capture or select images for recipes.
  - **Flashlight:** A hidden toggle for extra interactivity.
  - **Location Services:** Map nearby grocery stores with real-time location data.
  - **Haptic Feedback:** Provides tactile responses for enhanced interaction.
  - **Text-to-Speech:** Reads out recipe details for users with visual impairments.

- **Custom Navigation Controls:**  
  Consistent and intuitive navigation is achieved through custom controls like StadiumTopBar and StadiumBottomBar.

## Installation and Setup

1. **Clone the Repository:**

   ```bash
   git clone https://github.com/ManMetMobComp/mobcomp-assessment-Bcam2020.git
   cd mobcomp-assessment-Bcam2020

2. Open the Project in Visual Studio or Your Preferred IDE:
Ensure you have the latest .NET MAUI workload installed.

3. Restore NuGet Packages:
Visual Studio should automatically restore the necessary packages when you open the solution. If not, run:

bash
Copy
dotnet restore
4. Build and Run the App:
Select your target platform (Android, iOS, or Windows) and run the application.

## Usage

Home Page:
The home screen offers quick navigation to key sections like Ingredients, Recipes, Community, and Profile via custom navigation bars.

Recipe Creation:
On the Ingredients page, users add individual ingredients. The Add Recipe page allows users to submit a complete recipe with instructions and a photo.

Community Interaction:
The Community page displays recipes from all users. Tapping a recipe card navigates to details and comments, while interactive elements like comment buttons enhance engagement.

User Profile:
Manage your profile information, view your created recipes and favorites, and update your profile picture.

Map Feature:
The Map page uses your location to display nearby grocery stores on an interactive map.

## Page Behaviors and Rules

Ingredients Page:
Users can add and remove ingredients using intuitive input fields and buttons. Input validation ensures only valid data is accepted.

Add Recipe Page:
Includes validation for recipe name, ingredients, and instructions. Users can attach photos via the camera or gallery, with errors handled gracefully.

Community Page:
Displays recipe cards with user details, recipe information, images, and recent comments. Interactive elements allow users to view comments or navigate to further details.

Comments Page:
Enables users to view and post comments on a recipe. Input validation ensures that blank comments are not submitted.

## Firebase Integration and Data Handling

Authentication & Data Storage:
Firebase is used to securely manage user authentication and store data, including recipes and user profiles.

Firestore Services:
All CRUD operations for recipes, comments, and profiles are performed through Firestore services.

CSV Parsing:
A custom CSV loader parses sample recipe data, which populates the community feed.

## Accessibility

The app is built to be accessible:

Semantic Properties:
All controls have SemanticProperties.Description, SemanticProperties.Hint, and SemanticProperties.HeadingLevel set to assist screen readers.

Screen Reader Compliance:
The design follows WCAG guidelines, ensuring that users with disabilities can navigate and interact with the app efficiently.

Testing Accessibility:
Enable your device's screen reader (TalkBack on Android or VoiceOver on iOS) and navigate through the app to experience descriptive prompts for each control.

## Mobile Hardware Integration

Camera:
Capture or select images for recipes.

Flashlight:
A hidden flashlight toggle is available in the settings.

Location Services:
Uses real-time location data to display nearby grocery stores on the Map page.

Haptic Feedback:
Provides tactile responses on interactions such as button taps and gestures.

Text-to-Speech:
Reads out recipe details to enhance accessibility for visually impaired users.

## Code Structure and Quality

Separation of Concerns:
The UI is defined in XAML files, and the business logic is in code-behind files, promoting a clean and maintainable codebase.

Custom Controls:
Reusable controls like StadiumTopBar and StadiumBottomBar are used throughout the app for consistent navigation.

Dynamic Resources:
Colors, fonts, and themes are managed using dynamic resources, which ensures the app adapts easily to theming changes.

Robust Error Handling:
Comprehensive error handling and input validation are implemented to ensure a smooth user experience.

## GitHub Repository and Commit History

Regular Commits:
The repository is updated frequently with detailed commit messages, documenting progress and feature enhancements.

Comprehensive Documentation:
This README, along with additional documentation in the repository, provides a thorough overview of the project.

Version Control Best Practices:
A well-organized commit history and clear branching strategy demonstrate professional project management.

## License

This project is licensed under the MIT License.

## Contributors

Ben C. – https://github.com/Bcam2020

## References

Logo:
Logo created using Adobe Express.
https://www.adobe.com/express/create/logo

Icons:
Free icons obtained from Flaticon.
https://www.flaticon.com/
