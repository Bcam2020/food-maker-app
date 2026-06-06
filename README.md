# Food Maker App

A cross-platform mobile application built with .NET MAUI that allows users to browse, create, and share recipes within an interactive community. Designed for cooking enthusiasts of all levels, the app helps users find recipes based on the ingredients they have available, contribute their own creations, and engage with a community of fellow cooks. Built with a focus on responsive UI/UX, accessibility (WCAG-aligned), and native hardware integration.

## Tech Stack

- **.NET MAUI** — Cross-platform mobile framework (C#)
- **Firebase Authentication** — User login and registration
- **Cloud Firestore** — Real-time data storage for recipes, comments, and profiles
- **SQLite (sqlite-net-pcl)** — Local data persistence
- **XAML** — UI markup with dynamic theming

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
- [License](#license)
- [Contributors](#contributors)
- [References](#references)

## Project Description

Food Maker App is designed for cooking enthusiasts who may have limited ingredients at home yet still want to prepare delicious meals. The application provides a full-featured platform to:

- **Browse Recipes:** Explore a dynamic community feed tailored to varying ingredient availability.
- **Create and Manage Recipes:** Submit recipes with detailed instructions, ingredient lists, and photos — even with a single ingredient on hand.
- **Community Interaction:** Publish recipes, add friends, and comment on others' creations.
- **Hardware Integration:** Leverage native mobile features including camera, flashlight, location services, haptic feedback, and text-to-speech.
- **Dynamic Theming and Accessibility:** Dark/light modes, adjustable text sizes, and accessibility enhancements ensuring the app is usable for everyone.

## Features

- **Dynamic Theming & Responsive UI:** Uses dynamic resources for colors and fonts to support dark/light modes and adjustable text sizes across all device sizes.

- **Accessibility Enhancements:** Comprehensive use of SemanticProperties (Description, Hint, HeadingLevel) across every page and custom control, aligned with WCAG guidelines.

- **User Authentication:** Firebase Authentication for secure user login and registration.

- **Recipe Management & CSV Parsing:** Submit, edit, and delete recipes with detailed instructions and ingredient lists. A custom CSV parser loads sample recipe data so users with minimal ingredients still have options.

- **Community Interaction:** Real-time community feed where users can view, comment, and interact with recipes.

- **Device Compatibility:** Tested on Pixel Tablet and Pixel 7 Pro running Android API 35.

- **Mobile Hardware Integration:**
  - **Camera:** Capture or select images for recipes.
  - **Flashlight:** Hidden toggle in settings.
  - **Location Services:** Map nearby grocery stores with real-time location data.
  - **Haptic Feedback:** Tactile responses on key interactions.
  - **Text-to-Speech:** Reads out recipe details for users with visual impairments.

- **Custom Navigation Controls:** Reusable controls (StadiumTopBar, StadiumBottomBar) provide consistent navigation across the app.

## Installation and Setup

1. **Clone the Repository:**

```bash
   git clone https://github.com/Bcam2020/food-maker-app.git
   cd food-maker-app
```

2. **Open the Project in Visual Studio:** Ensure you have the latest .NET MAUI workload installed.

3. **Restore NuGet Packages:** Visual Studio will restore packages automatically. If not, run:

```bash
   dotnet restore
```

4. **Build and Run:** Select your target platform (Android, iOS, or Windows) and run the application. Firebase configuration files are required and not included in this repository — see the Firebase Integration section.

## Usage

**Home Page:** Quick navigation to Ingredients, Recipes, Community, and Profile via custom navigation bars.

**Recipe Creation:** On the Ingredients page, users add individual ingredients. The Add Recipe page allows users to submit a complete recipe with instructions and a photo.

**Community Interaction:** The Community page displays recipes from all users. Tapping a recipe card navigates to details and comments.

**User Profile:** Manage profile information, view created recipes and favourites, update profile picture.

**Map Feature:** The Map page uses location data to display nearby grocery stores on an interactive map.

## Page Behaviors and Rules

**Ingredients Page:** Users can add and remove ingredients using intuitive input fields and buttons. Input validation ensures only valid data is accepted.

**Add Recipe Page:** Includes validation for recipe name, ingredients, and instructions. Users can attach photos via camera or gallery, with errors handled gracefully.

**Community Page:** Displays recipe cards with user details, recipe information, images, and recent comments. Interactive elements allow users to view comments or navigate to further details.

**Comments Page:** Enables users to view and post comments on a recipe. Input validation prevents blank comment submissions.

## Firebase Integration and Data Handling

**Authentication & Data Storage:** Firebase securely manages user authentication and stores data including recipes and user profiles.

**Firestore Services:** All CRUD operations for recipes, comments, and profiles are performed through dedicated Firestore service classes.

**CSV Parsing:** A custom CSV loader parses sample recipe data which populates the community feed at startup.

## Accessibility

The app is built with accessibility as a first-class concern:

**Semantic Properties:** All controls have SemanticProperties.Description, SemanticProperties.Hint, and SemanticProperties.HeadingLevel set to assist screen readers.

**Screen Reader Compliance:** The design follows WCAG guidelines, ensuring users with disabilities can navigate and interact with the app efficiently.

**Testing Accessibility:** Enable your device's screen reader (TalkBack on Android or VoiceOver on iOS) and navigate through the app to experience descriptive prompts for each control.

## Mobile Hardware Integration

**Camera:** Capture or select images for recipes.

**Flashlight:** Hidden flashlight toggle available in settings.

**Location Services:** Real-time location data displays nearby grocery stores on the Map page.

**Haptic Feedback:** Tactile responses on button taps and gestures.

**Text-to-Speech:** Reads out recipe details to enhance accessibility for visually impaired users.

## Code Structure and Quality

**Separation of Concerns:** UI is defined in XAML files; business logic lives in code-behind and dedicated service classes.

**Custom Controls:** Reusable controls (StadiumTopBar, StadiumBottomBar) provide consistent navigation across the app.

**Dynamic Resources:** Colours, fonts, and themes are managed via dynamic resources, supporting seamless theme changes at runtime.

**Robust Error Handling:** Comprehensive error handling and input validation throughout the codebase.

## License

This project is licensed under the MIT License.

## Contributors

Ben Camphor – https://github.com/Bcam2020

## References

**Logo:** Created using Adobe Express — https://www.adobe.com/express/create/logo

**Icons:** Free icons obtained from Flaticon — https://www.flaticon.com/
