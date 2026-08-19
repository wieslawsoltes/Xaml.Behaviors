# DetachedFromVisualTreeTrigger

Executes when the associated object is detached from the visual tree.

The trigger executes before its behavior/action logical scope is released. Bound actions therefore retain their data context for the detach callback, including when content leaves the visual tree because a `TabControl` selection changes.
