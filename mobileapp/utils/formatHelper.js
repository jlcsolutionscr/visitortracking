import xmlParser from 'react-xml-parser'

export function formatCurrency (number, decPlaces, decSep, thouSep) {
  decPlaces = isNaN(decPlaces = Math.abs(decPlaces)) ? 2 : decPlaces,
  decSep = typeof decSep === "undefined" ? "." : decSep
  thouSep = typeof thouSep === "undefined" ? "," : thouSep
  const decIndex = number.toString().indexOf(decSep)
  const sign = number < 0 ? "-" : ""
  let decValue = decIndex > 0 ? number.toString().substring(1 + decIndex, 1 + decIndex + decPlaces) : ""
  if (decValue.length < decPlaces) decValue += '0'.repeat(decPlaces - decValue.length)
  const integerValue = decIndex > 0 ? number.toString().substring(0, decIndex) : number.toString()
  return sign + integerValue.replace(/(\d)(?=(\d{3})+(?!\d))/g, '$1,') + decSep + decValue
}

export function roundNumber(number, places) {
  return +(Math.round(number + "e+" + places) + "e-" + places)
}

export function arrayToString (byteArray) {
  return String.fromCharCode.apply(null, byteArray)
}

export function xmlToHtmlString (value) {
  const parser = new xmlParser()
  xml = parser.parseFromString(value)
  let html = '<!DOCTYPE html><html><body>'
  html += createHtmlElement(xml.name, xml.value, xml.children, 0)
  html += '</body></html>'
  return html
}

function createHtmlElement(name, value, children, level) {
  const tagStyle = 'font-family: monospace; font-size: 11px; color: rgb(136, 18, 128);'
  const textStyle = 'font-family: monospace; font-size: 11px;'
  if (name == 'ds:Signature') return ''
  let element = '<div style="margin-left: ' + level + 'em;"><span style="' + tagStyle + '">&lt;' + name + '&gt;</span>'
  if (value == '')
    element += '</div>'
  else {
    element += '<span style="' + textStyle + '">' + value + '</span><span style="' + tagStyle + '">&lt;' + name + '&gt;</span></div>'
  }
  if (children.length > 0) {
    children.forEach(child => {
      element += createHtmlElement(child.name, child.value, child.children, level + 1)
    })
  }
  return element
}
