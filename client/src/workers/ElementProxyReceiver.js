import {EventDispatcher} from 'three';
 
export class ElementProxyReceiver extends EventDispatcher {
    constructor() {
        super();
        this.style = {};
    }
    get clientWidth() {
        return this.width;
    }
    get clientHeight() {
        return this.height;
    }
    getRootNode() {
        return this;
    }
    getBoundingClientRect() {
        return {
        left: this.left,
        top: this.top,
        width: this.width,
        height: this.height,
        right: this.left + this.width,
        bottom: this.top + this.height,
        };
    }
    handleEvent(data) {
        if (data.type === 'size') {
        this.left = data.left;
        this.top = data.top;
        this.width = data.width;
        this.height = data.height;
        return;
        }
        data.preventDefault = noop;
        data.stopPropagation = noop;
        this.dispatchEvent(data);
    }
    focus() {}
    setPointerCapture() {}
    releasePointerCapture() {}
}

function noop() {
}
 

export class ProxyManager {
  constructor() {
    this.targets = {};
    this.handleEvent = this.handleEvent.bind(this);
  }
  makeProxy(data) {
    const {id} = data;
    const proxy = new ElementProxyReceiver();
    this.targets[id] = proxy;
  }
  getProxy(id) {
    return this.targets[id];
  }
  handleEvent(data) {
    this.targets[data.id].handleEvent(data.data);
  }
}