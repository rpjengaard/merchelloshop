function clone(obj) {
	if (!obj) {
		return null;
	}
	return JSON.parse(JSON.stringify(obj));
}

function forEachDeep(obj, childKey, callback) {
	if (obj) {
		callback(obj);
		if (obj[childKey] && Array.isArray(obj[childKey])) {
			obj[childKey].forEach((child) => {
				forEachDeep(child, childKey, callback);
			});
		}
	}
}

export default {
	forEachDeep,
	clone,
};
