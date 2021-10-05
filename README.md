# Get the fork out

A dotnet tool to delete GitHub forks for a user.

## Disclaimer

Do not run this application without fully knowing what it does.  
Deleted forks CAN NOT be recovered once deleted.

## Installing

```
> dotnet tool install -g get-the-fork-out
```

## Usage

```
USAGE:
    dotnet get-the-fork-out <GITHUB-API-KEY> [OPTIONS]

ARGUMENTS:
    <GITHUB-API-KEY>    The GitHub API key to use.
                        The API key must have the following permissions: 
                        public_repo, delete_packages   
OPTIONS:
    -h, --help                      Prints help information
        --i-know-what-i-am-doing    No prompts will be shown.
                                    WARNING: All forks will automatically be deleted
```

## Example 

```
❯ dotnet get-the-fork-out <MY-GITHUB-API-KEY>
┌────────────────────────┬─────────────────┬──────────────────────────────┐
│ Owner                  │ Repository      │ Last updated                 │
├────────────────────────┼─────────────────┼──────────────────────────────┤
│ patriksvensson         │ vscode          │ 2021-10-05 21:50:35          │
│ patriksvensson         │ kubernetes      │ 2021-10-05 21:50:45          │
└────────────────────────┴─────────────────┴──────────────────────────────┘
┌─WARNING─────────────────────────────────────────────────────────────────┐
│ This tool is used to DELETE forks owned by the user patriksvensson.     │
│ Deleted forks cannot be restored. Press CTRL+C to abort at any time.    │
└─────────────────────────────────────────────────────────────────────────┘
Are you sure that you want to continue? [y/n] (n): y

Delete patriksvensson/vscode [y/n] (n): y
✅ Deleted patriksvensson/vscode

Delete patriksvensson/kubernetes [y/n] (n): y
✅ Deleted patriksvensson/kubernetes
```