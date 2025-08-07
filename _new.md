# C# Coding Standards for NCacheTestClient

## General Coding Standards:
- Keep functions short and focused.
- Declare variables as close as possible to where they are used.
- Avoid macros that create custom programming languages.
- Do not use 'goto'.
- Avoid putting closing braces more than one screen away from their matching opening braces.

## Git Standards:
- Each commit should focus on a single type of change. For example, do not combine method additions and comments in the same commit.
- Example: Use separate commits like "Added methods for event handling" and "Added comments" instead of combining them.

## Commenting Standards:
- Write XML summary comments for every class and public method.
- Do not start comments with phrases like "This class..." or "This method...".

## Object-Oriented Programming (OOP):
- Always create a constructor if all properties must be initialized for a valid object.
- If a property always needs the same initial value (e.g., workingDaysOfWeek), initialize it outside the constructor to avoid errors in overloaded constructors.
- Use private properties and expose them as protected for use in inherited classes.

## Agile Practices:
- Story point 1 ≈ half a day’s work.
- Hierarchy: Epic → Feature → User Stories/Task.
- User stories should be written from the end-user perspective. For NCache, users include both developers (who use NCache in code) and admins/DevOps (who deploy and maintain NCache).