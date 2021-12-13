# Changelog

## 0.2.0

### New property grid

This release introduces a property grid that is a little nicer to look at. Booleans will now be proper check boxes and text and numbers will have get a text box.
When properties are editable the input controls in the property grid are now actually editable, for example the tag name.

To make this work properly, the models have been updated with the right attributes that drive the display in the property grid.

### Tag naming

You can now change the name of a tag. This helps greatly when you are analyzing a Protobuf payload and you want to see more than `tag1`, `tag2` etc. 
It's now possible to set them to any arbitrary value such as `foo`, `bar` or whatever suits your needs.

![screenshot showing tag rename feature](change-tag-name.png)

### Installer UI

The installer now shows more feedback of the installation process rather than having the plain MSI dialogs.
During installation the license is now also shown properly which was not the case before.