/**
 * Utility to help handle retrieving and setting data in LocalStorage
 * JavaScript Library independent
 */
var localStore = function(key, options) {
    this.options = {
        defaultVal: null,
        expires: null        
    }
    
    for(item in options) {
        this.options[item] = options[item];
    }
    
    this.key = key;
    this.defaultVal = this.options.defaultVal;
    this.expires = this.options.expires;
    
    var that = this;
    
    this.get = function() {
        var item = window.localStorage.getItem(that.key);
        if(that.expires && item){
        	//return null if expired
        	var now = new Date();
        	if(now >= that.expires) return null;
        }
        
        try {
            return item !== null ? JSON.parse(item) : (that.defaultVal ? that.defaultVal : null);
        }
        catch(e) {
            return item !== null ? item : (that.defaultVal ? that.defaultVal : null);
        }
    };
    
    this.set = function(value) {
        window.localStorage.setItem(that.key, JSON.stringify(value));
    };
    
    this.remove = function() {
        window.localStorage.removeItem(that.key);    
    };
};