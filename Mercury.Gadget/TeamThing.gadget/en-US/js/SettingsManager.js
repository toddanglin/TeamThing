/*	Gagdet Settings Manager - SettingsManager.js
Version 1.0, May 26, 2007
Author: Todd Northrop
Company: Speednet Group
Web Site: Lottery Post (http://www.lotterypost.com)
E-mail: webmaster (at) lotterypost (dot) com
You may use this code freely and without Author's permission, but copies and derivitive code must retain this unaltered comment banner at the top.
------------------------------------------------------*/

var SettingsManager = {
    /// <summary>
    /// Gagdet Settings Manager for Windows Vista Sidebar gadgets.
    /// </summary>

    FileName: "Settings.ini",
    LocalFolder: "",
    _validNamePattern: "\\w(?:[\\w \\-\\.~!@#\\$%\\^&\\*\\(\\)\\+:<>,/\\?]*\\w)?", // Regex specifying valid group and key names: Must be at least 1 char long; must start and end with letter, number, or underscore; can also contain spaces and punctuation.
    _groups: [],

    getFullPath: function () {
        /// <summary>
        /// Constructs and returns the full path of the gadget ini file on disk.
        /// </summary>
        /// <returns type="String">The full path of the ini file</returns>
        /// <remarks>
        /// The path consists of the root path of the gadget (as specified by the System.Gadget.path property), plus the LocalFolder (if specified), plus the FileName.
        /// </remarks>
        return System.Gadget.path + ((this.LocalFolder.length == 0) ? "" : "\\" + this.LocalFolder.replace("/", "\\").replace(/^(\s\\)+/, "").replace(/(\s\\)+$/, "")) + "\\" + this.FileName;
    },

    getValue: function (group, key, defaultValue) {
        /// <summary>
        /// getValue() retrieves a value from the ini settings in memory currently.
        /// </summary>
        /// <param name="group" type="String" mayBeNull="false" optional="false">Group name containing the value to retrieve</param>
        /// <param name="key" type="String" mayBeNull="false" optional="false">Key name of the value to retrieve</param>
        /// <param name="defaultValue" type="String" mayBeNull="true" optional="true">If the key is not found within the group specified, getValue() will return defaultValue, if specified, or an empty string if it is not specified or is null/undefined.</param>
        /// <returns type="String">The ini setting currently in memory, as specified by group and keys names.  Return an empty string ("") if <group, key> is not found and defaultValue is not specified.</returns>
        /// <remarks>
        /// Organize ini settings into groups (specified by the group parameter).  Within each group can be one or more key/value pairs.
        /// </remarks>

        if (!this.isValidName(group) || !this.isValidName(key)) {
            return "";
        }

        var numGroups = this._groups.length;
        var x, y;

        for (x = 0; x < numGroups; x++) {
            if (this._groups[x].Name == group) {
                break;
            }
        }

        if (x >= numGroups) {
            return defaultValue || "";
        }

        var vals = this._groups[x].Values;
        var numValues = vals.length;

        for (y = 0; y < numValues; y++) {
            if (vals[y].Key == key) {
                break;
            }
        }

        if (y >= numValues) {
            return defaultValue || "";
        }

        return vals[y].Value;
    },

    setValue: function (group, key, value) {
        /// <summary>
        /// setValue() adds or updates an ini setting in memory.  Any changes made to the ini settings in memory are not saved to disk until saveFile() is called.
        /// </summary>
        /// <param name="group" type="String" mayBeNull="false" optional="false">Group name in the ini settings to store the key/value pair</param>
        /// <param name="key" type="String" mayBeNull="false" optional="false">The same key name can be used in multiple groups, but within one group a key name can only be used once</param>
        /// <param name="value" type="String" mayBeNull="false" optional="false">The value to store in the ini settings</param>
        /// <remarks>
        /// Organize ini settings into groups (specified by the group parameter).  Within each group can be one or more key/value pairs, as specified by key and value parameters.
        /// </remarks>

        if (!this.isValidName(group) || !this.isValidName(key)) {
            return "";
        }

        var numGroups = this._groups.length;
        var x, y;

        for (x = 0; x < numGroups; x++) {
            if (this._groups[x].Name == group) {
                break;
            }
        }

        if (x >= numGroups) {
            this._groups[x] = { Name: group, Values: [{ Key: key, Value: value}] };
        }
        else {
            var vals = this._groups[x].Values;
            var numValues = vals.length;

            for (y = 0; y < numValues; y++) {
                if (vals[y].Key == key) {
                    break;
                }
            }

            vals[y] = { Key: key, Value: value };
        }
    },

    deleteValue: function (group, key, deleteEmptyGroup) {
        /// <summary>
        /// Removes a key/value pair from the ini settings in memory currently.  Any changes made to the ini settings in memory are not saved to disk until saveFile() is called.
        /// </summary>
        /// <param name="group" type="String" mayBeNull="false" optional="false">Group name containing the key/value pair to delete</param>
        /// <param name="key" type="String" mayBeNull="false" optional="false">Key name to delete</param>
        /// <param name="deleteEmptyGroup" type="Boolean" mayBeNull="true" optional="true">If the group is empty after deleting the key/value pair, this flag specifies if the empty group should be deleted also.  The default value of this parameter is true.</param>

        if (!this.isValidName(group) || !this.isValidName(key)) {
            return;
        }

        if (typeof (deleteEmptyGroup) != "boolean") {
            deleteEmptyGroup = true;
        }

        var numGroups = this._groups.length;
        var x, y;

        for (x = 0; x < numGroups; x++) {
            if (this._groups[x].Name == group) {
                break;
            }
        }

        if (x >= numGroups) {
            return;
        }

        var vals = this._groups[x].Values;
        var numValues = vals.length;

        for (y = 0; y < numValues; y++) {
            if (vals[y].Key == key) {
                vals.splice(y, 1);
                numValues--;
                break;
            }
        }

        if ((numValues <= 0) && (deleteEmptyGroup)) {
            this._groups.splice(x, 1);
        }
    },

    deleteGroup: function (group) {
        /// <summary>
        /// Removes a group of key/value pairs from the ini settings in memory currently.  Any changes made to the ini settings in memory are not saved to disk until saveFile() is called.
        /// </summary>
        /// <param name="group" type="String" mayBeNull="false" optional="false">Group name to delete</param>

        if (!this.isValidName(group)) {
            return;
        }

        var numGroups = this._groups.length;

        for (var x = 0; x < numGroups; x++) {
            if (this._groups[x].Name == group) {
                this._groups.splice(x, 1);
                break;
            }
        }
    },

    getKeyCount: function (group) {
        /// <summary>
        /// Returns the number of key/value pairs within the group specified.
        /// </summary>
        /// <param name="group" type="String" mayBeNull="false" optional="false">Group name containing the key/value pairs to count</param>
        /// <returns type="Number" integer="true">The number of groups that are in the ini settings in memory currently.</returns>

        if (!this.isValidName(group)) {
            return 0;
        }

        var numGroups = this._groups.length;
        var x;

        for (x = 0; x < numGroups; x++) {
            if (this._groups[x].Name == group) {
                break;
            }
        }

        if (x >= numGroups) {
            return 0;
        }

        return this._groups[x].Values.length;
    },

    getKeyNames: function (group) {
        /// <summary>
        /// Retrieves an array of all the keys names in one group in the ini settings in memory currently.
        /// </summary>
        /// <param name="group" type="String" mayBeNull="false" optional="false">Group name containing the key names to retrieve</param>
        /// <returns type="Array">An array of Strings, with each element being the name of a key in the specified group.</returns>

        if (!this.isValidName(group)) {
            return [];
        }

        var numGroups = this._groups.length;
        var x, y;

        for (x = 0; x < numGroups; x++) {
            if (this._groups[x].Name == group) {
                break;
            }
        }

        if (x >= numGroups) {
            return [];
        }

        var vals = this._groups[x].Values;
        var numValues = vals.length;
        var k = [];

        for (y = 0; y < numValues; y++) {
            k[y] = vals[y].Key;
        }

        return k;
    },

    getGroupCount: function () {
        /// <summary>
        /// Returns the number of group names in the ini settings in memory currently.
        /// </summary>
        /// <returns type="Number" integer="true">The number of groups that are in the ini settings in memory currently.</returns>
        return this._groups.length;
    },

    getGroupNames: function () {
        /// <summary>
        /// Retrieves an array of all the group names in the ini settings in memory currently.
        /// </summary>
        /// <returns type="Array">An array of Strings, with each element being the name of a group in the ini settings in memory currently.</returns>
        var numGroups = this._groups.length;
        var g = [];

        for (var x = 0; x < numGroups; x++) {
            g[x] = this._groups[x].Name;
        }

        return g;
    },

    saveFile: function () {
        /// <summary>
        /// saveFile() saves any changes made to the ini settings in memory since the last time loadFile() was called.  If the ini file does not exist it is created; otherwise it is over-written.
        /// </summary>
        try {
            var fs = new ActiveXObject("Scripting.FileSystemObject");
            var newFile = fs.CreateTextFile(this.getFullPath(), true);

            try {
                newFile.Write(this.getIniString());
            }
            finally {
                newFile.Close();
            }
        }
        catch (e) {
            // Do nothing
        }
    },

    getIniString: function () {
        /// <summary>
        /// getIniString() generates and returns a string representing the complete contents of the ini file, based upon the ini settings in memory.  The string does not represent what is stored in the ini file on disk, only what is in memory currently.
        /// </summary>
        /// <returns type="String">
        /// All the ini settings currently in memory, written in the ini file format
        /// </returns>
        /// <remarks>
        /// saveFile() calls this function, and then writes the output string from this function to the ini file on disk.
        /// </remarks>
        var now = new Date();
        var ini = "; Automatically generated by SettingsManager - the ini file manager for Vista gadgets\n; By Speednet - see http://gadgets.speednet.biz/ for more information.\n; " + now.toLocaleString() + "\n\n";
        var numGroups = this._groups.length;
        var vals, numValues, x, y;

        for (x = 0; x < numGroups; x++) {
            ini += "[" + this._groups[x].Name + "]\n";
            vals = this._groups[x].Values;
            numValues = vals.length;

            for (y = 0; y < numValues; y++) {
                ini += vals[y].Key + '="' + escape(vals[y].Value) + '"\n';
            }

            ini += "\n";
        }

        return ini;
    },

    loadFile: function () {
        /// <summary>
        /// loadFile() reads the contents of that gadget ini file into memory.  Any changes made to the ini settings in memory are not saved to disk until saveFile() is called.
        /// </summary>
        try {
            var fs = new ActiveXObject("Scripting.FileSystemObject");

            try {
                var ini = "";
                var ts = fs.OpenTextFile(this.getFullPath(), 1); // 1 = ForReading

                try {
                    ini = ts.ReadAll();
                }
                finally {
                    ts.Close();
                }

                if (ini.length == 0) {
                    this._groups = [];
                }
                else {
                    var regexGroup = new RegExp('^[ \\t]*\\[(' + this._validNamePattern + ')\\][ \\t]*(?:[;#].*)?$', 'm');
                    var regexValue = new RegExp('^[ \\t]*(' + this._validNamePattern + ')[ \\t]*=[ \\t]*"([^"]*)"[ \\t]*(?:[;#].*)?$', 'm');
                    var lines = ini.match(/^(?![ \\t]*[;#])[ \\t]*\S.*$/mg);
                    var lineCount = lines.length;
                    var n = 0;
                    var groups = [];
                    var group = -1;
                    var values, result, advance;

                    while (n < lineCount) {
                        regexGroup.lastIndex = 0;
                        advance = true;

                        if ((result = regexGroup.exec(lines[n])) != null) {
                            group = groups.length;
                            groups[group] = { Name: result[1], Values: [] };
                            values = groups[group].Values;
                            advance = false;
                            n++;
                        }

                        if (group >= 0) {
                            regexValue.lastIndex = 0;

                            if ((result = regexValue.exec(lines[n])) != null) {
                                values[values.length] = { Key: result[1], Value: unescape(result[2]) };
                                advance = true;
                            }
                        }

                        if (advance) {
                            n++;
                        }
                    }

                    this._groups = groups;
                }
            }
            finally {
                fs = null;
            }
        }
        catch (e) {
            // Do nothing
        }
    },

    isValidName: function (name) {
        /// <summary>
        /// Determines if the name passed is valid, based on the RegExp pattern in _validNamePattern.  The same pattern is used for both group and key names.
        /// </summary>
        /// <returns type="Boolean">Returns true if the name passed is valid, otherwise returns false.</returns>
        var r = new RegExp("^" + this._validNamePattern + "$");
        return r.test(name);
    }
};
	