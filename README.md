# HinduCyborg

Repository for Hindu Cyborg in Space game made with Unity 5.5.2f1

ITIA9 Game Project course at University of Tampere, 2017
Team: Hindu Cyborgs

-------------------------
To use Git:
- Download and install Git https://git-scm.com/downloads
- Make a Github account
- A handy Git guide: https://services.github.com/on-demand/downloads/github-git-cheat-sheet.pdf
- Use an advanced Git GUI: https://www.sourcetreeapp.com/ (optional, but might make life easier)
- Adding an SSH key to keep your sanity with Git Bash: https://help.github.com/articles/connecting-to-github-with-ssh/

With Git Bash:

  Configure Git (optional but recommended):
  - git config --global user.name "[your name]"
  - git config --global user.email "[your email address]"

  Cloning the project to your PC (use HTTPS if you use a SourceTree, SSH only if you've generated an SSH key):
  - HTTPS: git clone https://github.com/RowEchelonForm/HinduCyborg.git
  OR
  - SSH: git clone git@github.com:RowEchelonForm/HinduCyborg.git

  Before you start to your work:
  - git pull
  (pulls the changes from the global repository master branch)

  After making changes to the project and stuff:
  - git add [file_name]
  OR
  - git add --all
  (this adds the file(s) to be tracked by Git)
  AND
  - git commit -m "your commit message"
  (this will commit the changes to YOUR LOCAL repository)
  AND
  - git push
  (this will push the changes to THE GLOBAL repository)
  
  If something weird happens when you push, there are probably changes in the global repository and you must pull them first. Then you will need to merge the files if there are conflicts (Git will merge automatically if there are no confilcts). If you think this is confusing, use a GUI, e.g. SourceTree. If you hate typing in your username and password all the time, use SSH or a GUI.
