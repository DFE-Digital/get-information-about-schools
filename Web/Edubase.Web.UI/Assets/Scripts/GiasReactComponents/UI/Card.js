import React from 'react';

const Card = props => {
  return (
    <div className={props.classes}>{props.children}</div>
  )
}

export default Card;
