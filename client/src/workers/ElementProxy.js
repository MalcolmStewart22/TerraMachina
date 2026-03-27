let nextProxyId = 0;

export class ElementProxy {
  constructor(element, worker, eventHandlers) {
    this.id = nextProxyId++;
    this.worker = worker;
    const sendEvent = (data) => {
      this.worker.postMessage({
        type: 'event',
        id: this.id,
        data,
      });
    };
 
    worker.postMessage({ type: 'makeProxy', id: this.id, });
    
    sendSize();
    
    for (const [eventName, handler] of Object.entries(eventHandlers)) {
            element.addEventListener(eventName, function(event) {
                handler(event, sendEvent);
            });
        }

    function sendSize() {
      const rect = element.getBoundingClientRect();
      sendEvent({
        type: 'size',
        left: rect.left,
        top: rect.top,
        width: element.clientWidth,
        height: element.clientHeight,
      });
    }
 
    window.addEventListener('resize', sendSize);

    
    }
}

export function makeSendPropertiesHandler(properties) {
    return function sendProperties(event, sendFn) {
        const data = {type: event.type};
        copyProperties(event, properties, data);
        sendFn(data);
    };
}

function copyProperties(src, properties, dst) {
  for (const name of properties) {
      dst[name] = src[name];
  }
}

function wheelEventHandler(event, sendFn) {
  event.preventDefault();
  wheelEventHandlerImpl(event, sendFn);
}
 
function preventDefaultHandler(event) {
  event.preventDefault();
}

function filteredKeydownEventHandler(event, sendFn) {
  const {keyCode} = event;
  if (orbitKeys[keyCode]) {
    event.preventDefault();
    keydownEventHandler(event, sendFn);
  }
}

const mouseEventHandler = makeSendPropertiesHandler([
  'ctrlKey',
  'metaKey',
  'shiftKey',
  'button',
  'pointerType',
  'clientX',
  'clientY',
  'pointerId',
  'pageX',
  'pageY',
]);

const wheelEventHandlerImpl = makeSendPropertiesHandler([
  'deltaX',
  'deltaY',
]);

const keydownEventHandler = makeSendPropertiesHandler([
  'ctrlKey',
  'metaKey',
  'shiftKey',
  'keyCode',
]);

const orbitKeys = {
  '37': true,  // left
  '38': true,  // up
  '39': true,  // right
  '40': true,  // down
};

export const eventHandlers = {
    contextmenu: preventDefaultHandler,
    mousedown: mouseEventHandler,
    mousemove: mouseEventHandler,
    mouseup: mouseEventHandler,
    pointerdown: mouseEventHandler,
    pointermove: mouseEventHandler,
    pointerup: mouseEventHandler,
    wheel: wheelEventHandler,
    keydown: filteredKeydownEventHandler,
};


