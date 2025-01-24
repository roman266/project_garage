# project_garage

## Branch Naming Conventions

To ensure consistency and clarity in our development process, we follow a structured naming convention for Git branches. Please adhere to these guidelines when creating new branches:

### Branch Types and Prefixes

Use the appropriate prefix to categorize your branch:

- **`feature/`**: For new features or enhancements.  
  _Example_: `feature/login-form`

- **`bugfix/`**: For resolving reported bugs.  
  _Example_: `bugfix/fix-login-redirect`

- **`hotfix/`**: For critical fixes in production.  
  _Example_: `hotfix/critical-payment-bug`

- **`refactor/`**: For code improvements or restructuring.  
  _Example_: `refactor/optimize-query-performance`

- **`docs/`**: For adding or updating documentation.  
  _Example_: `docs/update-readme`

- **`test/`**: For experimental or testing branches.  
  _Example_: `test/try-new-lib`

### General Guidelines

1. **Use Descriptive Names**: Clearly convey the purpose of the branch.

   - **Good**: `feature/add-search-bar`
   - **Bad**: `feature1`

2. **Include Task Identifiers**: Reference the issue or task ID when applicable.  
   _Example_: `feature/123-add-user-authentication`

3. **Separate Words with Hyphens**: For better readability.

   - **Good**: `bugfix/fix-header-alignment`
   - **Bad**: `bugfix_fixHeader`

4. **Use Lowercase**: Stick to lowercase letters to avoid conflicts on case-sensitive systems.

   - **Good**: `feature/add-logging`
   - **Bad**: `Feature/AddLogging`

5. **Avoid Special Characters**: Use only alphanumeric characters, `-`, or `/`.

   - **Good**: `feature/update-dashboard`
   - **Bad**: `feature/update@dashboard`

6. **Keep It Concise**: Make branch names short yet meaningful. Avoid overly long names.

   - **Good**: `refactor/cleanup-code`
   - **Bad**: `refactor/cleanup-unused-and-unnecessary-code-files`

7. **Avoid Personal or Temporary Names**: Names should reflect the task, not the individual or temporary purpose.
   - **Good**: `feature/user-profile`
   - **Bad**: `temp-branch`, `johns-fix`
