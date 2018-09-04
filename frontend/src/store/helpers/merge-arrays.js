export default (a, b) => a.concat(b.filter(item => a.indexOf(item) < 0));
