# GitHub Copilot

- GitHub Copilot is an AI-powered code completion tool that helps developers write code faster and with fewer errors. It uses machine learning to suggest code snippets, functions, and even entire files based on the context of the code being written.
- It integrates with popular code editors like Visual Studio Code, JetBrains IDEs, and others.
- These are its main features:
  - **Code Suggestions:** Provides real-time code suggestions as you type, helping to speed up the coding process.
  - **Context Generation:** Understands the context of your code and suggests relevant snippets, functions, or even entire files.
  - **Chat Interface:** Allows users to interact with Copilot in a chat-like interface, asking questions or requesting code snippets.
  - **Agent Mode:** Allows for more advanced interactions where Copilot can perform tasks based on user instructions.
- Installation depends on the code editor being used, but generally involves installing an extension or plugin, check the [GitHub Copilot documentation](https://docs.github.com/en/copilot) for specific instructions.

## Usage

- **Code Completion:**
  - Start typing code, and Copilot will suggest completions based on the context.
  - You can accept suggestions by pressing `Tab` or accept a word by pressing `CTRL + Tab` in Windows or `CMD + Tab` in macOS.
  - If there are multiple suggestions, you navigate between them using the arrow icons that appear or their shortcuts.
  - Make sure to have `Next Edit Suggestions` enabled to get suggestions of the next edit based on the current context.
- **Code Generation:**
  - One great approach is to write the pseudo-code or comments describing what you want to achieve, and Copilot will generate the corresponding code gradually instead of writing a chunk of code at once.
- **Chat Interface:**
  - **Inline Chat:** You can ask Copilot questions or request code snippets directly in the editor, just select the code you want to discuss and click on the Copilot icon to start a chat.
    - You can ask for explanations, request modifications, or even ask for alternative implementations, some of the pre-defined commands include (check them by adding `@` at the beginning of your prompt):
      - `/doc`: Generates documentation for the selected code.
      - `/edit`: Edits the selected code.
      - `/explain`: Explains the selected code.
      - `/fix`: Fixes issues in the selected code.
      - `/generate`: Generates code based on the selected code or context.
      - `/tests`: Generates tests for the selected code.
    - Remember to complement the command with user instructions to get the best results.
    - Alternatively, you can upload files or speak to Copilot using the microphone to ask questions or request code snippets.
    - If the answer is not satisfactory, you can refine your question, ask for more details, or even use a different model to get a different perspective.
    - When it gives you a suggestion, the highlighted code is the one that will be modified, the lighter selection is the code that was already there.
    - This also works in the terminal, allowing you to ask Copilot to generate and run terminal commands based on the context of your code or project.
  - **Code Actions:**
    - Alternatively to the inline chat, you can use the **Code Actions** (`CTRL + .` in Windows or `CMD + .` in macOS) to perform actions on the selected code, such as `Modify Using Copilot` or `Generate Using Copilot`, which will open a chat interface with Copilot to discuss the selected code.
  - **Chat Window:**
    - This is for more complex interactions, you can open the Copilot chat window by clicking on the Copilot icon in the editor or using the shortcut `CTRL + SHIFT + P` in Windows or `CMD + SHIFT + P` in macOS, then type `Copilot: Open Chat`, or use `CTRL + ALT + I` in Windows or `CMD + ALT + I` in macOS.
    - **Ask Mode:**
      - Here you'll get a more robust chat interface that will allow you to:
        - **Multi-file Context:** Copilot can understand and generate code across multiple files, making it easier to work on larger projects, the only thing you need to do is to add the files you want to include in the context, either by dragging and dropping them into the chat window or using the `Add File` button, or by writing the file name in the chat `#file:filename`.
        - **Get detailed explanations:** You can ask Copilot to explain code snippets, functions, or entire files, providing insights into how the code works.
        - **Get different options to implement the generated code:** With the suggestion you'll have a stack of actions to choose from:
          - Insert in Editor: Inserts the generated code directly into the editor.
          - Insert at Cursor: Inserts the generated code at the current cursor position.
          - Copy: Copies the generated code to the clipboard for use elsewhere.
          - Insert into Terminal: Inserts the generated code into the terminal, useful for command-line operations.
          - Insert into File: Inserts the generated code into a specific file, allowing for better organization of code.
      - **@** also works in the chat window, allowing you to use more commands and add more specific context to your requests, such as `@vscode` or `@terminal` to get terminal commands.
    - **Edit Mode:**
      - This mode allows you to add several files to the context, and it will generate code based on the context of those files, not only the current file but it'll also consider the creation of new files if necessary.
    - **Agent Mode:**
      - This mode allows Copilot to perform tasks based on user instructions, such as generating code, running commands, or even interacting with external APIs.
      - You can enable Agent Mode by clicking on the Copilot icon in the editor and selecting `Enable Agent Mode`.
      - Once enabled, you can give Copilot instructions like "Create a new file" or "Run this command" and it will execute those tasks for you.

## Tunning

- **GitHub › Copilot › Chat › Code Generation: Use Instruction Files** [`1]
  - You can create instruction files to guide Copilot's code generation process. These files can include specific instructions, coding standards, or examples that Copilot should follow when generating code.
  - To use instruction files, create a file named `.github/copilot-instructions.md` in your project directory and add your instructions there.
  - Copilot will automatically read this file and apply the instructions when generating code.
- Use **GitHub › Copilot › Chat › Editor › Temporal Context:** to control how much context Copilot uses when generating code. This can help improve the relevance of suggestions based on the current file or project.

[1]: Poor instructions can lead to worse results, so it's important to provide clear and concise instructions in the instruction files.

## References

- [GitHub Copilot Documentation](https://docs.github.com/en/copilot)
